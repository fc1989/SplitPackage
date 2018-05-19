import Util from '@/libs/util'
import qs from 'qs';

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/LogisticChannel/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/LogisticChannel/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/LogisticChannel/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/LogisticChannel/Delete?Id=' + id)
    },
    Verify(data){
        return Util.ajax.post('/api/services/app/LogisticChannel/Verify',data);
    },
    Query(flag,ids){
        return Util.ajax.post('/api/services/app/LogisticChannel/Query',{flag:flag,ids:ids});
    },
    Get(id){
        return Util.ajax.get('/api/services/app/LogisticChannel/Get?Id=' + id);
    },
    GetOptions(){
        return Util.ajax.get('/api/services/app/PublicInformation/GetLogisticChannelsOptions');
    },
    GetImportState(){
        return Util.ajax.get('/api/services/app/LogisticChannel/GetImportState');
    },
    CustomerImport(arrayKey){
        return Util.ajax.post('/api/services/app/LogisticChannel/CustomerImport',arrayKey);
    }
}