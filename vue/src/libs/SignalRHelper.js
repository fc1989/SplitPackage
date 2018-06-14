import AppConsts from './appconst'
export class SignalRHelper {
    static initSignalR(){

        jQuery.getScript(window.OperatingEnvironment.remoteServiceBaseUrl + '/signalr/hubs', () => {

            $.connection.hub.url = window.OperatingEnvironment.remoteServiceBaseUrl + "/signalr";

            var encryptedAuthToken = new UtilsService().getCookieValue(AppConsts.authorization.encrptedAuthTokenName);
            $.connection.hub.qs = AppConsts.authorization.encrptedAuthTokenName + "=" + encodeURIComponent(encryptedAuthToken);

            jQuery.getScript('/dist/abp/abp.signalr.js');
        });
    }
}