import Util from '@/libs/util'

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
    }
}