// @ts-nocheck

import { EventBus, EventHandler } from '../../../application/adapters/events/event-bus.js';
import { NatsConnection, StringCodec } from 'nats.ws';
import { logger } from '../../../common/services/logging/logger.js';

/**
 * @description Represents a standard implementation of an `EventBus` that used NATS.
 * @export
 * @class NatsEventsBus
 * @typedef {NatsEventsBus}
 * @implements {EventBus}
 */
export class NatsEventsBus implements EventBus {
    /**
     * @description Represents the underlying NATS connection.
     * @private
     * @readonly
     * @type {NatsConnection}
     */
    private readonly connection: NatsConnection;

    /**
     * @description The codec used when encoding and decoding messages sent over NATS.
     * @private
     * @readonly
     * @type {StringCodec}
     */
    private readonly codec: StringCodec;

    /**
     * Creates an instance of `NatsEventsBus`.
     * @constructor
     * @param {NatsConnection} connection The underlying NATS connection used when publishing and subscribing to messages.
     */
    constructor(connection: NatsConnection) {
        this.connection = connection;
        this.codec = StringCodec();
    }

    /**
     * @async
     * @inheritdoc
     */
    async publish<TData>(subject: string, data: TData): Promise<void> {
        this.connection.publish(subject, this.codec.encode(JSON.stringify(data)));
    }

    /**
     * @async
     * @inheritdoc
     */
    subscribe<TData>(subject: string, handler: EventHandler<TData>): () => void {
        const subscription = this.connection.subscribe(subject);

        (async () => {
            for await (const message of subscription) {
                try {
                    await handler(JSON.parse(this.codec.decode(message.data)));
                } catch (error) {
                    logger.error('Error Processing NATS Subscription Handler:', error);
                }
            }
        })();

        return () => subscription.unsubscribe();
    }
}
