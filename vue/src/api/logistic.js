import Util from '@/libs/util'

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
    }
}