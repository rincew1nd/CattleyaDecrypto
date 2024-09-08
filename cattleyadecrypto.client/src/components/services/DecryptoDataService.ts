import { type DecryptoMatch, DecryptoTeamEnum } from '../types/DecryptoTypes'
import {ref} from "vue";

interface UserInfo {
    id: string,
    name: string
}

export interface SubmitCluesRequest {
    matchId: string,
    team: DecryptoTeamEnum,
    order: number[],
    clues: string[]
}

export interface SolveCluesRequest {
    matchId: string,
    team: DecryptoTeamEnum,
    order: number[]
}

class DecryptoDataService {
    
    constructor() {
        this.login(null).then(r => {});
    }

    public userAuthData = ref<UserInfo | null>(null);
    
    async login(name: string | null): Promise<void> {
        let response = await fetch(`/api/authentication/login?${name != null ? `name=${name}` : ''}`, {method: "POST"});
        let json: any = await response.json();
        this.userAuthData.value = json as UserInfo;
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
        return response.status === 200;
    }
    
    async assignRiddler(id: string): Promise<boolean> {
        let response = await fetch(`/api/decrypto/assign-riddler?matchId=${id}`, {method: "POST"});
        return response.status === 200;
    }

    async submitClues(request:SubmitCluesRequest) {
        let response = await fetch(
            `/api/decrypto/submit-clues`,
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request)
            });
        return response.status === 200;
    }

    async solveClues(request:SolveCluesRequest) {
        let response = await fetch(
            `/api/decrypto/solve-clues`,
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request)
            });
        return response.status === 200;
    }

    async intercept(request:SolveCluesRequest) {
        let response = await fetch(
            `/api/decrypto/intercept`,
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request)
            });
        return response.status === 200;
    }
}

export default new DecryptoDataService();