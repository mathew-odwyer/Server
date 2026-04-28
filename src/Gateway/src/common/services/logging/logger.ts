import pino from 'pino';

/**
 * @description Represents the logger used throughout the application.
 * @type {pino.Logger<never, boolean>}
 */
export const logger = pino({
    level: process.env['LOG_LEVEL'] || 'debug',
});
