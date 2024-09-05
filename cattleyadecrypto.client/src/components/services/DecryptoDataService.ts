import { type DecryptoMatch, DecryptoTeamEnum } from '../types/DecryptoTypes'

interface AuthInfo {
    userId: string,
    name: string
}

export interface GiveCluesRequest {
    matchId: string,
    team: DecryptoTeamEnum,
    order: number[],
    clues: string[]
}

class DecryptoDataService {
    private authInfo:AuthInfo | null = null;
    
    constructor() {
        this.login().then(r => {});
    }

    getPlayerId(): string { return this.authInfo?.userId ?? ''; }
    getPlayerName(): string { return this.authInfo?.name ?? ''; }
    
    async login(): Promise<void> {
        let response = await fetch('/api/authentication/login', {method: "POST"});
        let json: any = await response.json();
        this.authInfo = json as AuthInfo;
    }

    async changeName(name: string): Promise<void> {
        let response = await fetch(`/api/authentication/changeName?name=${name}`, {method: "POST"});
        let json: any = await response.json();
        this.authInfo = json as AuthInfo;
    }
    
    async createMatch(): Promise<DecryptoMatch> {
        let response = await fetch('/api/decrypto/create-match', {method: "POST"});
        let json: any = await response.json();
        return json as DecryptoMatch;
    }

    async getMatch(id: string): Promise<DecryptoMatch> {
        let response = await fetch(`/api/decrypto/get-match?matchId=${id}`);
        let json: any = await response.json();
        return json as DecryptoMatch;
    }

    async joinTeam(id: string, team: DecryptoTeamEnum): Promise<boolean> {
        let response = await fetch(`/api/decrypto/join-team?matchId=${id}&team=${team}`, {method: "POST"});
        let json: any = await response.json();
        return json as boolean;
    }

    async submitClues(request:GiveCluesRequest) {
        let response = await fetch(
            `/api/decrypto/give-clues`,
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request)
            });
        let json: any = await response.json();
        return json as boolean;
    }
}

export default new DecryptoDataService();