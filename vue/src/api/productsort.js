import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/ProductSort/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/ProductSort/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/ProductSort/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/ProductSort/Delete?Id='+id)
    }
}