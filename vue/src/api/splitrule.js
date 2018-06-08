import Util from '@/libs/util'
import qs from 'qs';

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/SplitRule/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/SplitRule/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/SplitRule/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/SplitRule/Delete?Id='+id)
    },
    Get(id){
        return Util.ajax.get('/api/services/app/SplitRule/Get?Id=' + id);
    },
    Switch(id, IsActive){
        return Util.ajax.post('/api/services/app/SplitRule/Switch',qs.stringify({id:id,IsActive:IsActive}));
    }
}