import Util from '@/libs/util'
import qs from 'qs';

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/LogisticLine/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/LogisticLine/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/LogisticLine/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/LogisticLine/Delete?Id='+id)
    },
    Verify(code){
        return Util.ajax.post('/api/services/app/LogisticLine/Verify',qs.stringify({codesku:code}));
    },
    Query(flag,ids){
        return Util.ajax.post('/api/services/app/LogisticLine/Query',{flag:flag,ids:ids});
    }
}