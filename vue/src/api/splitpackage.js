import Util from '@/libs/util'

export default {
    SplitOrder(data){
        return Util.ajax.post('/api/SplitPackage/Split',data);
    },
    SplitOrderByAssign(data){
        return Util.ajax.post('/api/SplitPackage/SplitWithExp1',data);
    }
}