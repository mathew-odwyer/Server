// @ts-nocheck

import { EventBus, EventHandler } from "../../../application/adapters/events/event-bus.js";
import { NatsConnection, StringCodec } from "nats.ws";
import { logger } from "../../../common/services/logging/logger.js";

export class NatsEventsBus implements EventBus {
    private readonly connection:NatsConnection;

    private readonly codec:StringCodec;

    constructor(connection: NatsConnection) {
        this.connection = connection;
        this.codec = StringCodec();
    }

    async publish<TData>(subject: string, data: TData): Promise<void> {
        this.connection.publish(subject, this.codec.encode(JSON.stringify(data)));
    }

    subscribe<TData>(subject: string, handler: EventHandler<TData>): () => void {
        const subscription = this.connection.subscribe(subject);

        (async () => {
            for await (const message of subscription) {
                try {
                    await handler(JSON.parse(this.codec.decode(message.data)));
                } catch (error) {
                    logger.error("Error Processing NATS Subscription Handler:", error);
                }
            }
        })();

        return () => subscription.unsubscribe();
    }
}
