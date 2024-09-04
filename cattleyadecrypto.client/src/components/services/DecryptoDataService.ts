import { type DecryptoMatch, DecryptoTeam } from '../types/DecryptoTypes'

interface AuthInfo {
    userId: string,
    name: string
}

class DecryptoDataService {
    private authInfo:AuthInfo | null = null;
    
    constructor() {
        this.login().then(r => {});
    }

    getPlayerId(): string | undefined { return this.authInfo?.userId; }
    getPlayerName(): string | undefined { return this.authInfo?.name; }
    
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

    async joinTeam(id: string, team: DecryptoTeam): Promise<boolean> {
        let response = await fetch(`/api/decrypto/join-team?matchId=${id}&team=${team}`, {method: "POST"});
        let json: any = await response.json();
        return json as boolean;
    }
}

export default new DecryptoDataService();