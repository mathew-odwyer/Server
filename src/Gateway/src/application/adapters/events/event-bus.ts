export type EventHandler<TData> = (data: TData) => void | Promise<void>;

export interface EventBus {
    publish<TData>(subject: string, data: TData): Promise<void>;
    
    subscribe<TData>(subject: string, handler: EventHandler<TData>): () => void;
}
