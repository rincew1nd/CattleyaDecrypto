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
    roundClues: Record<DecryptoTeamEnum, DecryptoTemporaryClue>;
}

export interface DecryptoTeam {
    miscommunicationCount: number;
    interceptionCount:     number;
    words:                 string[];
    clues:                 Record<number, string[]> | null;
    players:               Record<string, string>;
}

export interface DecryptoTemporaryClue {
    order:         number[];
    clues:         Record<number, string>;
    riddlerId:     string;
    isSolved:      boolean;
    isIntercepted: boolean;
}

export interface DecryptoPlayerEvent {
    playerId:   string;
    playerName: string;
    team:       DecryptoTeamEnum;
}

export interface DecryptoSensitiveInfoEvent {
    words:          string[];
    roundWordOrder: number[];
}