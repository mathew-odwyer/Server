import { Cache, CacheSetOptions, CacheResult } from '../../../application/adapters/data/cache.js';
import { RedisClientType, SetOptions } from 'redis';

/**
 * @description Converts a `CacheSetOptions` to `SetOptions`.
 * @param {?CacheSetOptions} [options] The cache options to be converted.
 * @returns {(SetOptions | undefined)} Returns the converted options, or `undefined`.
 */
function toRedisSetOptions(options?: CacheSetOptions): SetOptions | undefined {
    if (!options) {
        return undefined;
    }

    if ('keepTtl' in options) {
        return {
            expiration: {
                type: 'KEEPTTL',
            },
        };
    }

    return {
        expiration: {
            type: 'EX',
            value: options.ttl,
        },
    };
}

/**
 * @description Represents a standard implementation of `Cache` that uses Redis.
 * @export
 * @class RedisCache
 * @typedef {RedisCache}
 * @implements {Cache}
 */
export class RedisCache implements Cache {
    /**
     * @description The redis client used to handle memory operations.
     * @private
     * @readonly
     * @type {RedisClientType}
     */
    private readonly redis: RedisClientType;

    /**
     * @description Creates an instance of `RedisCache`.
     * @constructor
     * @param {RedisClientType} redis The redis client used to handle memory operations.
     */
    constructor(redis: RedisClientType) {
        this.redis = redis;
    }

    /**
     * @async
     * @inheritdoc
     */
    async get<TData>(key: string): Promise<CacheResult<TData>> {
        const data = await this.redis.get(key);

        if (data === null) {
            return {
                exists: false,
            };
        }

        const value = JSON.parse(data) as TData;

        return {
            exists: true,
            value: value,
        };
    }

    /**
     * @async
     * @inheritdoc
     */
    async set<TData>(key: string, value: TData, options?: CacheSetOptions): Promise<void> {
        const data = JSON.stringify(value);
        const redisOptions = toRedisSetOptions(options);

        await this.redis.set(key, data, redisOptions);
    }
}
