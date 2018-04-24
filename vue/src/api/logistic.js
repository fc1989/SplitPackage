import Util from '@/libs/util'
import qs from 'qs';

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/Logistic/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/Logistic/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/Logistic/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/Logistic/Delete?Id='+id)
    },
    Verify(flag){
        return Util.ajax.post('/api/services/app/Logistic/Verify',qs.stringify({flag:flag}));
    },
    Query(flag,ids){
        return Util.ajax.post('/api/services/app/Logistic/Query',{flag:flag,ids:ids});
    }
}