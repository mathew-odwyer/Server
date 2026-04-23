export type CacheResult<T> =
  | { exists: true; value: T }
  | { exists: false };

export type CacheSetOptions =
  | { ttl: number }
  | { keepTtl: true };

export interface Cache {
    set<TData>(key: string, value: TData, options?: CacheSetOptions): Promise<void>;
    get<TData>(key:string): Promise<CacheResult<TData>>;
}
