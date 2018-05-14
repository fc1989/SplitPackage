import util from '@/libs/util';
import Cookies from 'js-cookie';

const session={
    namespaced: true,
    state:{
        application:null,
        user:null,
        tenant:null,
        tenantId:null
    },
    mutations:{
        setTenantId (state) {
            state.tenantId = Cookies.get("Abp.TenantId") === undefined ? null : Cookies.get("Abp.TenantId");
        }
    },
    actions:{
        async init({state}){
            let rep=await util.ajax.get('/api/services/app/Session/GetCurrentLoginInformations',{
                headers:{
                    'Abp.TenantId': abp.multiTenancy.getTenantIdCookie()
                }});
            state.application=rep.data.result.application;
            state.user=rep.data.result.user;
            state.tenant=rep.data.result.tenant;
        }
    }
}
export default session;