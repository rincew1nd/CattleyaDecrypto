import {ref} from "vue";

import DecryptoDataService from '../services/DecryptoDataService'
import DecryptoMessageService from '../services/DecryptoMessageService'

import type {DecryptoMatch} from "@/components/types/DecryptoTypes";
import {DecryptoTeam} from "@/components/types/DecryptoTypes";

export const MatchInfo = ref<DecryptoMatch | null>(null);
export const Team = ref<DecryptoTeam>(DecryptoTeam.Unknown);
export const OppositeTeam = ref<DecryptoTeam>(DecryptoTeam.Unknown);

function updateMatch(match:DecryptoMatch) {
    MatchInfo.value = match;

    let userId = DecryptoDataService.getPlayerId();
    Object.entries(MatchInfo.value.teams).forEach(([key1, val1]) => {
        Object.entries(val1.players).forEach(([key2, val2]) => {
            if (key2 === userId) {
                Team.value = key1 === 'red' ? DecryptoTeam.Red : DecryptoTeam.Blue;
            }
        })
    });
    
    OppositeTeam.value = (Team.value === DecryptoTeam.Unknown
        ? DecryptoTeam.Unknown : (Team.value === DecryptoTeam.Blue ? DecryptoTeam.Red : DecryptoTeam.Blue)) as DecryptoTeam;
    
    if (OppositeTeam.value != DecryptoTeam.Unknown)
    {
        MatchInfo.value.teams[OppositeTeam.value].words = ['#', '#', '#', '#']
    }
    else
    {
        MatchInfo.value.teams[1].words = ['#', '#', '#', '#']
        MatchInfo.value.teams[2].words = ['#', '#', '#', '#']
    }

    console.log('Match state updated', MatchInfo, Team);
}

export async function joinMatch(id: string): Promise<void> {
    try {
        let match = await DecryptoDataService.getMatch(id);
        
        updateMatch(match);

        await DecryptoMessageService.start();

        await DecryptoMessageService.connection.invoke("JoinMatch", id);

        DecryptoMessageService.connection.on("StateChanged", (match: DecryptoMatch) => {
            updateMatch(match);
        });
        
        console.log('Successfully connected to match')
    } catch (err) {
        console.error('Error fetching match data:', err);
    }
}
