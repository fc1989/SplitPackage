import Util from '@/libs/util'
import qs from 'qs';

export default {
    Update(data){
        return Util.ajax.put('/api/services/app/ProductClass/Update',data);
    },
    Create(data){
        return Util.ajax.post('/api/services/app/ProductClass/Create',data);
    },
    Search(data){
        return Util.ajax.get('/api/services/app/ProductClass/GetAll',data);
    },
    Delete(id){
        return Util.ajax.delete('/api/services/app/ProductClass/Delete?Id='+id)
    },
    VerifyPTId(productSortId, ptid){
        return Util.ajax.post('/api/services/app/ProductClass/VerifyPTId',qs.stringify({productSortId:productSortId,ptid:ptid}));
    },
    VerifyClassName(productSortId, className){
        return Util.ajax.post('/api/services/app/ProductClass/VerifyClassName',qs.stringify({productSortId:productSortId, className:className}));
    },
    GetOptional(){
        return Util.ajax.get('/api/services/app/ProductClass/GetOptional');
    }
}