import {computed, ref} from "vue";
import {
    DecryptoTeamEnum,
    DecryptoMatchState,
    type DecryptoTeam,
    type DecryptoMatch,
    type DecryptoTemporaryClue,
    type DecryptoPlayerEvent,
    type DecryptoAssignPlayerEvent,
} from "@/components/types/DecryptoTypes";

import DecryptoDataService from '../services/DecryptoDataService'
import DecryptoMessageService from '../services/DecryptoMessageService'

export const Team = ref<DecryptoTeamEnum>(DecryptoTeamEnum.Unknown);
export const OppositeTeam = ref<DecryptoTeamEnum>(DecryptoTeamEnum.Unknown);
export const MatchInfo = ref<DecryptoMatch | null>(null);

export const NeedRiddler = computed(() => 
    !MatchInfo.value?.temporaryClues[Team.value]);
export const IsRiddler = computed(() =>
    MatchInfo.value?.temporaryClues[Team.value]?.riddlerId == DecryptoDataService.getPlayerId());

function updateMatch(match:DecryptoMatch) {
    MatchInfo.value = match;
    calculateSides(match);
    hideWords(match)
    console.log('Match state updated', MatchInfo, Team);
}

function hideWords(match:DecryptoMatch) {
    if (OppositeTeam.value != DecryptoTeamEnum.Unknown) {
        match.teams[OppositeTeam.value].words = ['#', '#', '#', '#']
    }
}

function calculateSides(match:DecryptoMatch) {
    let userId = DecryptoDataService.getPlayerId();
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
        if (event === DecryptoMatchState.GiveClues) {
            MatchInfo.value.temporaryClues = {} as Record<DecryptoTeamEnum, DecryptoTemporaryClue>;
        }
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
        MatchInfo.value.temporaryClues = event;
    }
}

function assignClueGiver(event:DecryptoAssignPlayerEvent) {
    if (MatchInfo.value) {
        MatchInfo.value.temporaryClues[event.team] = {
            riddlerId: event.playerId,
            order: event.order
        } as DecryptoTemporaryClue;
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

        await DecryptoMessageService.connection.invoke("JoinMatch", id);

        DecryptoMessageService.connection.on("StateChanged", (event: DecryptoMatchState) => {
            matchStateUpdated(event);
        });
        DecryptoMessageService.connection.on("PlayerJoined", (event: DecryptoPlayerEvent) => {
            playerJoined(event);
        });
        DecryptoMessageService.connection.on("AssignRiddler", (event: DecryptoAssignPlayerEvent) => {
            assignClueGiver(event);
        });
        DecryptoMessageService.connection.on("SolveClues", (event: Record<DecryptoTeamEnum, DecryptoTemporaryClue>) => {
            prepareClues(event);
        });
        
        console.log('Successfully connected to match')
    } catch (err) {
        console.error('Error fetching match data:', err);
    }
}
