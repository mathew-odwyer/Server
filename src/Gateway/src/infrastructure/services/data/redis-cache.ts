import { Cache, CacheSetOptions, CacheResult } from "../../../application/adapters/data/cache.js";
import { RedisClientType, SetOptions } from "redis";

function toRedisSetOptions(options?: CacheSetOptions): SetOptions | undefined {
    if (!options) {
        return undefined;
    }

    if ("keepTtl" in options) {
        return {
            expiration: {
                type: "KEEPTTL",
            },
        };
    }

    return {
        expiration: {
            type: "EX",
            value: options.ttl,
        },
    };
}

export class RedisCache implements Cache {
    private readonly redis:RedisClientType;

    constructor(redis:RedisClientType) {
        this.redis = redis;
    }

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

    async set<TData>(key: string, value: TData, options?: CacheSetOptions): Promise<void> {
        const data = JSON.stringify(value);
        const redisOptions = toRedisSetOptions(options);

        await this.redis.set(key, data, redisOptions);
    }
}