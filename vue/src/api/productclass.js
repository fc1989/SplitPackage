﻿import Util from '@/libs/util'

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
    }
}