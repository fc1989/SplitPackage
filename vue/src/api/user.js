import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/User/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/User/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/User/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/User/Delete?Id='+id)
    },
    GetAllRoles(){
        return Util.ajax.get('/api/services/app/User/GetRoles');;
    }
}