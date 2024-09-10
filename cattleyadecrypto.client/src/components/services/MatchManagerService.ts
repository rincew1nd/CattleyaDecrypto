import {computed, ref} from "vue";
import {
    type DecryptoMatch,
    DecryptoMatchState,
    type DecryptoPlayerEvent,
    type DecryptoSensitiveInfoEvent,
    DecryptoTeamEnum,
    type DecryptoTemporaryClue,
    type PrepareClue,
    type ScoreAndStateUpdate
} from "@/components/types/DecryptoTypes";

import DecryptoDataService from '../services/DecryptoDataService'
import DecryptoMessageService from '../services/DecryptoMessageService'

export const Team = ref<DecryptoTeamEnum>(DecryptoTeamEnum.Unknown);
export const OppositeTeam = ref<DecryptoTeamEnum>(DecryptoTeamEnum.Unknown);
export const MatchInfo = ref<DecryptoMatch | null>(null);

export const NeedRiddler = computed(() => 
    !MatchInfo.value?.roundClues[Team.value]);
export const IsRiddler = computed(() =>
    MatchInfo.value?.roundClues[Team.value]?.riddlerId == DecryptoDataService.userAuthData.value?.id);

function updateMatch(match:DecryptoMatch) {
    MatchInfo.value = match;
    calculateSides(match);
    hideWords(match);
    console.log('Match state updated', MatchInfo, Team);
}

function changePlayerName(event: DecryptoPlayerEvent) {
    if (MatchInfo.value) {
        MatchInfo.value.teams[event.team].players[event.playerId] = event.playerName;
    }
}

function playerJoined(event:DecryptoPlayerEvent) {
    if (MatchInfo.value?.teams[event.team]) {
        MatchInfo.value.teams[event.team].players[event.playerId] = event.playerName;
        calculateSides(MatchInfo.value);
        hideWords(MatchInfo.value);
    }
}

function setSensitiveInfo(event: DecryptoSensitiveInfoEvent) {
    if (MatchInfo.value) {
        MatchInfo.value.teams[Team.value].words = event.words;
        MatchInfo.value.roundClues[Team.value].order = event.roundWordOrder;
    }
}

function matchStateUpdated(event:DecryptoMatchState) {
    if (MatchInfo.value) {
        MatchInfo.value.state = event;
        if (event === DecryptoMatchState.GiveClues) {
            MatchInfo.value.roundClues = {} as Record<DecryptoTeamEnum, DecryptoTemporaryClue>;
        }
    }
}

function assignClueGiver(event:DecryptoPlayerEvent) {
    if (MatchInfo.value) {
        MatchInfo.value.roundClues[event.team] = {
            riddlerId: event.playerId,
        } as DecryptoTemporaryClue;
    }
}

function prepareClues(event: PrepareClue) {
    if (MatchInfo.value) {
        MatchInfo.value.roundClues[event.team].clues = event.clues;
    }
}

function scoreAndStateUpdate(event: ScoreAndStateUpdate) {
    if (MatchInfo.value) {
        MatchInfo.value.teams[event.team].interceptionCount = event.interceptions;
        MatchInfo.value.teams[event.team].miscommunicationCount = event.miscommunications;
        if (event.matchState === DecryptoMatchState.SolveClues) {
            MatchInfo.value.roundClues[event.team].isSolved = true;
        }
        if (event.matchState === DecryptoMatchState.Intercept) {
            MatchInfo.value.roundClues[getOppositeTeam(event.team)].isIntercepted = true;
        }
    }
}

function cluesUpdate(event: Record<string, Record<number, string[]>>) {
    if (MatchInfo.value) {
        MatchInfo.value.teams[DecryptoTeamEnum.Red].clues = event[DecryptoTeamEnum.Red];
        MatchInfo.value.teams[DecryptoTeamEnum.Blue].clues = event[DecryptoTeamEnum.Blue];
    }
}

// Private methods to update model

function hideWords(match:DecryptoMatch) {
    if (OppositeTeam.value != DecryptoTeamEnum.Unknown) {
        match.teams[OppositeTeam.value].words = ['1', '2', '3', '4']
    }
}

function calculateSides(match:DecryptoMatch) {
    let userId = DecryptoDataService.userAuthData.value?.id;
    Object.entries(match.teams).forEach(([key, val]) => {
        if (Object.keys(val.players).includes(userId!)) {
            Team.value = key === '1' ? DecryptoTeamEnum.Red : DecryptoTeamEnum.Blue;
        }
    });
    OppositeTeam.value = getOppositeTeam(Team.value);
}

function getOppositeTeam(team:DecryptoTeamEnum) {
    return team === DecryptoTeamEnum.Unknown
        ? DecryptoTeamEnum.Unknown
        : team === DecryptoTeamEnum.Blue
            ? DecryptoTeamEnum.Red
            : DecryptoTeamEnum.Blue;
}

function matchFinished(event: DecryptoTeamEnum) {
    if (MatchInfo.value) {
        MatchInfo.value.wonTeam = event;
        MatchInfo.value.state = DecryptoMatchState.Finished;
    }
}

export async function joinMatch(id: string): Promise<void> {
    try {
        MatchInfo.value = null;
        Team.value = DecryptoTeamEnum.Unknown;
        OppositeTeam.value = DecryptoTeamEnum.Unknown;
        
        let match = await DecryptoDataService.getMatch(id);
        
        updateMatch(match);

        await DecryptoMessageService.start();

        DecryptoMessageService.connection.on("StateChanged", (event: DecryptoMatchState) => {
            matchStateUpdated(event);
        });
        DecryptoMessageService.connection.on("PlayerJoined", (event: DecryptoPlayerEvent) => {
            playerJoined(event);
        });
        DecryptoMessageService.connection.on("SensitiveInfo", (event: DecryptoSensitiveInfoEvent) => {
            setSensitiveInfo(event);
        });
        DecryptoMessageService.connection.on("AssignRiddler", (event: DecryptoPlayerEvent) => {
            assignClueGiver(event);
        });
        DecryptoMessageService.connection.on("SolveClues", (event: PrepareClue) => {
            prepareClues(event);
        });
        DecryptoMessageService.connection.on("ScoreAndStateUpdate", (event: ScoreAndStateUpdate) => {
            scoreAndStateUpdate(event);
        });
        DecryptoMessageService.connection.on("CluesUpdate", (event: Record<string, Record<number, string[]>>) => {
            cluesUpdate(event);
        });
        DecryptoMessageService.connection.on("NameChanged", (event: DecryptoPlayerEvent) => {
            changePlayerName(event);
        });
        DecryptoMessageService.connection.on("MatchFinished", (event: DecryptoTeamEnum) => {
           matchFinished(event); 
        });

        await DecryptoMessageService.connection.invoke("JoinMatch", id);
        
        console.log('Successfully connected to match')
    } catch (err) {
        console.error('Error fetching match data:', err);
    }
}
