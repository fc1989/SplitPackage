import Util from '@/libs/util'

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/SplitRuleItem/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/SplitRuleItem/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/SplitRuleItem/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/SplitRuleItem/Delete?Id='+id)
    },
    Get(id){
        return Util.ajax.get('/api/services/app/SplitRuleItem/Get?Id=' + id);
    }
}