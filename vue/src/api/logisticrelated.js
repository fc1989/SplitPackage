import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/LogisticRelated/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/LogisticRelated/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/LogisticRelated/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/LogisticRelated/Delete?Id='+id);
    },
    Get(id){
        return Util.ajax.get('/api/services/app/LogisticRelated/Get?Id='+id)
    }
}