import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/WeightFreight/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/WeightFreight/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/WeightFreight/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/WeightFreight/Delete?Id='+id)
    }
}