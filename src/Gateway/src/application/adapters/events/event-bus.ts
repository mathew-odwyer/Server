/**
 * @description Defines a type that represents an event handler used when handling an event.
 * @export
 * @typedef {EventHandler}
 * @template TData The data that is contained within the event (ie; the event data).
 */
export type EventHandler<TData> = (data: TData) => void | Promise<void>;

/**
 * @description Defines an interface that represents an event bus.
 * @export
 * @interface EventBus
 * @typedef {EventBus}
 */
export interface EventBus {
    /**
     * @description Publishes an event of the specified `subject`.
     * @template TData The data contained within the event.
     * @param {string} subject The subject (or name) of the event to be published.
     * @param {TData} data The data contained within the event (ie; the event data).
     * @returns {Promise<void>} Returns a promise that is resolved when the operation has completed.
     */
    publish<TData>(subject: string, data: TData): Promise<void>;

    /**
     * @description Subscribes to an event of the specified `subject`.
     *
     * @template TData The data contained within the event.
     * @param {string} subject The subject (or name) of the event to be subscribed to.
     * @param {EventHandler<TData>} handler The handler that is executed when the event occurs.
     * @returns {() => void} Returns a function that can be used to unsubscribe from the event.
     */
    subscribe<TData>(subject: string, handler: EventHandler<TData>): () => void;
}
