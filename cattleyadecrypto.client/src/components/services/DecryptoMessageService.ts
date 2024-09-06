import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";

class DecryptoMessageService {
    public connection = new HubConnectionBuilder()
        .withUrl("/api/decryptoMessageHub")
        .withAutomaticReconnect()
        .build();
    
    public async start() {
        try {
            if (this.connection.state !== HubConnectionState.Connected) {
                await this.connection.start().then(() => {
                    this.setupListeners();
                });
            }
        } catch (err) {
            console.error(err);
            setTimeout(this.start, 5000);
        }
    }

    private setupListeners() {
        this.connection.on("SignalRTest", (datetime: string) => {
            console.log(`Current server time - ${datetime}`);
        });
    }
}

export default new DecryptoMessageService();