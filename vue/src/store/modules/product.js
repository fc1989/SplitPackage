import Util from '@/libs/util'

const product = {
    namespaced:true,
    state: {
        products:[],
        totalCount:0,
        pageSize:10,
        currentPage:1
    },
    mutations: {
        setPageSize(state,size){
            state.pageSize=size;
        },
        setCurrentPage(state,page){
            state.currentPage=page;
        }
    },
    actions:{
        async getAll({state},payload){
            let page={
                maxResultCount:state.pageSize,
                skipCount:(state.currentPage-1)*state.pageSize
            }
            let rep= await Util.ajax.get('/api/services/app/Product/GetAll',{params:page});
            state.products=[];
            state.products.push(...rep.data.result.items);
            state.totalCount=rep.data.result.totalCount;
        },
        async delete({state},payload){
            await Util.ajax.delete('/api/services/app/Product/Delete?Id='+payload.data.id);
        },
        async create({state},payload){
            await Util.ajax.post('/api/services/app/Product/Create',payload.data);
        },
        async update({state},payload){
            await Util.ajax.put('/api/services/app/Product/Update',payload.data);
        }
    }
};

export default product;
