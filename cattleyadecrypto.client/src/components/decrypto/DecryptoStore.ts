import {type DecryptoMatch, DecryptoMatchState, DecryptoTeamEnum, type DecryptoTeam} from '../types/DecryptoTypes'

export const DecryptoStore: DecryptoMatch = {
    id: "match12345",
    round: 2,
    state: DecryptoMatchState.GiveClues,
    wonTeam: undefined,
    teams: {
        [DecryptoTeamEnum.Red]: {
            miscommunicationCount: 1,
            interceptionCount: 0,
            words: {
                1: "apple",
                2: "banana",
                3: "cherry",
                4: "peach"
            },
            clues: {
                1: ["fruit"],
                2: ["yellow", "long"],
                3: ["stone", "summer", "red"],
                4: []
            },
            players: ["Alice", "Bob", "Charlie"]
        },
        [DecryptoTeamEnum.Blue]: {
            miscommunicationCount: 0,
            interceptionCount: 1,
            words: {
                1: "dog",
                2: "cat",
                3: "bird",
                4: "fish"
            },
            clues: {
                1: ["pet"],
                2: ["whiskers", "claws"],
                3: ["feathers", "wings", "sky"],
                4: ["scales"]
            },
            players: ["David", "Eve", "Frank"]
        }
    },
    temporaryClues: {
        [DecryptoTeamEnum.Red]: {
            order: [1, 2, 3],
            clues: {
                1: "fruit",
                2: "yellow",
                3: "stone"
            },
            isSolved: false,
            isIntercepted: false
        },
        [DecryptoTeamEnum.Blue]: {
            order: [1, 2, 3],
            clues: {
                1: "pet",
                2: "whiskers",
                3: "feathers"
            },
            isSolved: false,
            isIntercepted: false
        }
    }
};