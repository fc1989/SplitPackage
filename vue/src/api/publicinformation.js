import Util from '@/libs/util'

export default {
    GetEnumOptions(){
        return Util.ajax.get('/api/services/app/PublicInformation/GetEnumOptions');
    }
}