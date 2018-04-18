import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/NumFreight/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/NumFreight/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/NumFreight/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/NumFreight/Delete?Id='+id)
    }
}