import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/Tenant/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/Tenant/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/Tenant/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/Tenant/Delete?Id='+id)
    }
}