import { HubConnectionBuilder } from "@microsoft/signalr";
import type {DecryptoMatch} from "@/components/types/DecryptoTypes";

export class DecryptoMessageService {
    private callback: ((value: DecryptoMatch) => void);
    
    connection = new HubConnectionBuilder()
        .withUrl("/api/decryptoMessageHub")
        .withAutomaticReconnect()
        .build();
    
    public async start() {
        try {
            await this.connection.start().then(() => {
                this.setupListeners();
            });
        } catch (err) {
            console.error(err);
            setTimeout(this.start, 5000);
        }
    }

    private setupListeners() {
        this.connection.on("StateChanged", (match: DecryptoMatch) => {
            this.handleMatchUpdate(match);
        });
        this.connection.on("SignalRTest", (datetime: string) => {
            console.log(`Current server time - ${datetime}`);
        });
    }

    private handleMatchUpdate(match: DecryptoMatch) {
        this.callback(match);
    }

    public async joinDecryptoMatch(id: string, callback: (value: DecryptoMatch) => void) {
        try {
            await this.connection.invoke("JoinMatch", id);
            this.callback = callback;
        } catch (err) {
            console.error(err);
            setTimeout(this.start, 5000);
        }
    }
}