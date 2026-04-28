/**
 * @description Defines a type that represents a cached result.
 * @export
 * @typedef {CacheResult}
 * @template T The type of data that has been cached.
 */
export type CacheResult<T> = { exists: true; value: T } | { exists: false };

/**
 * @description Defines a type that represents a collection of options for caching data.
 * @export
 * @typedef {CacheSetOptions}
 */
export type CacheSetOptions = { ttl: number } | { keepTtl: true };

/**
 * @description Defines an interface that represents an in-memory data store.
 * @export
 * @interface Cache
 * @typedef {Cache}
 */
export interface Cache {
    /**
     * @description Sets a key-value pair to be cached in memory.
     *
     * @template TData Defines the data type to be stored.
     * @param {string} key The key to be used when accessing the data.
     * @param {TData} value The data to be stored.
     * @param {?CacheSetOptions} [options] The cache options (optional).
     * @returns {Promise<void>} Returns a promise that is resolved when the operation has completed.
     */
    set<TData>(key: string, value: TData, options?: CacheSetOptions): Promise<void>;

    /**
     * @description Gets data stored in-memory via its key.
     * @template TData Defines the data type to be retrieved.
     * @param {string} key The key used to fetch the data.
     * @returns {Promise<CacheResult<TData>>} Returns a promise that contains the cached result.
     */
    get<TData>(key: string): Promise<CacheResult<TData>>;
}
