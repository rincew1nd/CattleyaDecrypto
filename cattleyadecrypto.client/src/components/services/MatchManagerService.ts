import {computed, ref} from "vue";
import {
    DecryptoTeamEnum,
    DecryptoMatchState,
    type DecryptoMatch,
    type DecryptoTemporaryClue,
    type DecryptoPlayerEvent,
    type DecryptoSensitiveInfoEvent
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

function hideWords(match:DecryptoMatch) {
    if (OppositeTeam.value != DecryptoTeamEnum.Unknown) {
        match.teams[OppositeTeam.value].words = ['#', '#', '#', '#']
    }
}

function calculateSides(match:DecryptoMatch) {
    let userId = DecryptoDataService.userAuthData.value?.id;
    Object.entries(match.teams).forEach(([key, val]) => {
        if (Object.keys(val.players).includes(userId)) {
            Team.value = key === '1' ? DecryptoTeamEnum.Red : DecryptoTeamEnum.Blue;
        }
    });
    OppositeTeam.value = Team.value === DecryptoTeamEnum.Unknown
        ? DecryptoTeamEnum.Unknown
        : Team.value === DecryptoTeamEnum.Blue
            ? DecryptoTeamEnum.Red
            : DecryptoTeamEnum.Blue;
}

function matchStateUpdated(event:DecryptoMatchState) {
    if (MatchInfo.value) {
        MatchInfo.value.state = event;
    }
}

function playerJoined(event:DecryptoPlayerEvent) {
    if (MatchInfo.value?.teams[event.team]) {
        MatchInfo.value.teams[event.team].players[event.playerId] = event.playerName;
        calculateSides(MatchInfo.value);
        hideWords(MatchInfo.value);
    }
}

function prepareClues(event: Record<DecryptoTeamEnum, DecryptoTemporaryClue>) {
    if (MatchInfo.value) {
        MatchInfo.value.roundClues = event;
    }
}

function assignClueGiver(event:DecryptoPlayerEvent) {
    if (MatchInfo.value) {
        MatchInfo.value.roundClues[event.team] = {
            riddlerId: event.playerId,
        } as DecryptoTemporaryClue;
    }
}

function setSensitiveInfo(event: DecryptoSensitiveInfoEvent) {
    if (MatchInfo.value) {
        MatchInfo.value.teams[Team.value].words = event.words;
        MatchInfo.value.roundClues[Team.value].order = event.roundWordOrder;
    }
}

function changePlayerName(event: DecryptoPlayerEvent) {
    if (MatchInfo.value) {
        MatchInfo.value.teams[event.team].players[event.playerId] = event.playerName;
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
        DecryptoMessageService.connection.on("SolveClues", (event: Record<DecryptoTeamEnum, DecryptoTemporaryClue>) => {
            prepareClues(event);
        });
        DecryptoMessageService.connection.on("NameChanged", (event: DecryptoPlayerEvent) => {
            changePlayerName(event);
        });

        await DecryptoMessageService.connection.invoke("JoinMatch", id);
        
        console.log('Successfully connected to match')
    } catch (err) {
        console.error('Error fetching match data:', err);
    }
}
