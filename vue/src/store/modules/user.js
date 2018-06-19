import Cookies from 'js-cookie';
import Util from '@/libs/util';
import appconst from '@/libs/appconst';
import Vue from 'vue';

const user = {
    namespaced:true,
    mutations: {
        logout(){
            abp.auth.clearToken();
            location.reload();
        },
        setPageSize(state,size){
            state.pageSize=size;
        },
        setCurrentPage(state,page){
            state.currentPage=page;
        }
    },
    actions:{
        async login({state},payload){
            let rep=await Util.ajax.post("/api/TokenAuth/Authenticate",payload.data);
            var tokenExpireDate = payload.data.rememberMe ? (new Date(new Date().getTime() + 1000 * rep.data.result.expireInSeconds)) : undefined;
            abp.auth.setToken(rep.data.result.accessToken,tokenExpireDate);
            abp.utils.setCookieValue(appconst.authorization.encrptedAuthTokenName,rep.data.result.encryptedAccessToken,tokenExpireDate,abp.appPath)
        },
        async changeLanguage({state},payload){
            let rep=await Util.ajax.post('/api/services/app/User/ChangeLanguage',payload.data);
            abp.utils.setCookieValue(
                'Abp.Localization.CultureName',
                payload.data.languageName,
                new Date(new Date().getTime() + 5 * 365 * 86400000),
                abp.appPath
            );
            window.location.reload();
        }
    }
};

export default user;