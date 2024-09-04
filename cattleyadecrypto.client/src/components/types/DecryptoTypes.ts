export enum DecryptoMatchState {
    WaitingForPlayers = 0,
    GiveClues = 1,
    SolveClues = 2,
    Intercept = 3,
    Finished = 4,
}

export enum DecryptoTeam {
    Unknown = 0,
    Red = 1,
    Blue = 2
}

export interface DecryptoMatch {
    id:             string;
    round:          number;
    state:          DecryptoMatchState;
    wonTeam?:       DecryptoTeam;
    teams:          { [key in DecryptoTeam]: Team };
    temporaryClues: { [key in DecryptoTeam]: TemporaryClue };
}

export interface Team {
    miscommunicationCount: number;
    interceptionCount:     number;
    words:                 { [key: number]: string };
    clues:                 { [key: number]: string[] };
    players:               { [key: string]: string };
}

export interface TemporaryClue {
    order:         number[];
    clues:         { [key: number]: string };
    isSolved:      boolean;
    isIntercepted: boolean;
}
