import { reactive } from 'vue'

export const DecryptoStore = reactive({
    roundNumber: 0,
    teams: [
        {
            name: 'Red',
            players: ['Petya', 'Igor', 'Lexa'],
            interceptions: 2,
            miscommunications: 4,
            words: {
                "test1": ['Clue 1', 'Clue 2'],
                "test2": [],
                "test3": ['Clue 3'],
                "test4": ['Clue 4', 'Clue 5', 'Clue 6']
            }
        },
        {
            name: 'Blue',
            players: ['Vasya', 'Booba', 'Keka'],
            interceptions: 3,
            miscommunications: 5,
            words: {
                "test1": [],
                "test2": [],
                "test3": [],
                "test4": []
            }
        }
    ]
})