import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/Product/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/Product/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/Product/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/Product/Delete?Id='+id)
    }
}