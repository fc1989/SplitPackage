import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/Role/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/Role/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/Role/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/Role/Delete?Id='+id)
    },
    GetAllPermissions(){
        return Util.ajax.get('/api/services/app/Role/GetAllPermissions');
    }
}