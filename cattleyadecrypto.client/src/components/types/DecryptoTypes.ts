export enum DecryptoMatchState {
    WaitingForPlayers = 0,
    GiveClues = 1,
    SolveClues = 2,
    Intercept = 3,
    Finished = 4,
}

export enum DecryptoTeamEnum {
    Unknown = 0,
    Red = 1,
    Blue = 2
}

export interface DecryptoMatch {
    id:             string;
    round:          number;
    state:          DecryptoMatchState;
    wonTeam?:       DecryptoTeamEnum;
    teams:          Record<DecryptoTeamEnum, DecryptoTeam>;
    temporaryClues: Record<DecryptoTeamEnum, DecryptoTemporaryClue>;
}

export interface DecryptoTeam {
    miscommunicationCount: number;
    interceptionCount:     number;
    words:                 Record<number, string>;
    clues:                 Record<number, string[]>;
    players:               Record<string, string>;
}

export interface DecryptoTemporaryClue {
    order:         number[];
    clues:         Record<number, string>;
    riddler:       string;
    isSolved:      boolean;
    isIntercepted: boolean;
}

export interface DecryptoPlayerJoined {
    playerId:   string;
    playerName: string;
    team:       DecryptoTeamEnum;
}