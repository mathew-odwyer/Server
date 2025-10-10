import axios from 'axios';

/**
 * @type {import('axios').AxiosInstance}
 * @description The axios instance.
*/
const api = axios.create({
    baseURL: "http://web:8080",
    headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * @typedef {import('axios').AxiosResponse} AxiosResponse
 */

/**
 * @description Represents a login request data transfer object.
 * @typedef {Object} LoginUserRequestDto
 * @property {String} username - The username.
 * @property {String} password - The password.
 */

/**
 * @description Represents a login request data transfer object.
 * @typedef {Object} LoginUserResponseDto
 * @property {String} accessToken - The access token.
 * @property {String} refreshToken - The refresh token.
 */

/**
 * @description Represents a login request data transfer object.
 * @typedef {Object} RegisterUserRequestDto
 * @property {String} username - The username.
 * @property {String} password - The password.
 * @property {String} emailAddress - The email address.
 */

/**
 * @exports
 * @class UserAccountClient
 * @description The user account client used to make request to the user account API.
 */
export class UserAccountClient
{
    /**
     * @static
     * @description The user login URL.
     * @memberof UserAccountClient
     */
    static _loginUrl = '/api/UserAccount/Login';
    
    /**
     * @static
     * @description The user register URL.
     * @memberof UserAccountClient
     */
    static _registerUrl = '/api/UserAccount/Register';

    /**
     * @static
     * @description Sends a login request data transfer object to the API.
     * @param {LoginUserRequestDto} dto The login user request data transfer object.
     * @returns {Promise<AxiosResponse<LoginUserResponseDto>>} A promise that resolves with the Axios response of the login request.
     * @memberof UserAccountClient
     */
    static Login(dto)
    {
        
        return api.post(this._loginUrl, dto);
    }

    /**
     * @static
     * @description Sends a register request data transfer object to the API.
     * @param {RegisterUserRequestDto} dto The register user request data transfer object.
     * @returns {Promise<AxiosResponse>} A promise that resolves with the Axios response of the register request.
     * @memberof UserAccountClient
     */
    static Register(dto)
    {
        return api.post(this._loginUrl, dto);
    }
}