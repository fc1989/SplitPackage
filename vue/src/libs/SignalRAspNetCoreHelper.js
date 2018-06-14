import AppConsts from './appconst'
class SignalRAspNetCoreHelper{
    initSignalR(){
        var encryptedAuthToken = abp.utils.getCookieValue(AppConsts.authorization.encrptedAuthTokenName);

        abp.signalr = {
            autoConnect: true,
            connect: undefined,
            hubs: undefined,
            qs: AppConsts.authorization.encrptedAuthTokenName + "=" + encodeURIComponent(encryptedAuthToken),
            url: window.OperatingEnvironment.remoteServiceBaseUrl + '/signalr'
        };

        jQuery.getScript('/dist/abp/abp.signalr-client.js');
    }
}
export default new SignalRAspNetCoreHelper();