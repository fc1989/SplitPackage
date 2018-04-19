<template>
    <div>
        <Card>
            <p slot="title">{{$t('Products')}}</p>
            <Dropdown slot="extra"  @on-click="handleClickActionsDropdown">
                <a href="javascript:void(0)">
                    {{$t('Public.Actions')}}
                    <Icon type="android-more-vertical"></Icon>
                </a>
                <DropdownMenu slot="list">
                    <DropdownItem name='Refresh'>{{$t('Public.Refresh')}}</DropdownItem>
                    <DropdownItem name='Create'>{{$t('Public.Create')}}</DropdownItem>
                </DropdownMenu>
            </Dropdown>
            <Table :columns="columns" border :data="products"></Table>
            <Page :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </Card>
        <Modal v-model="showModal" :title="$t('Public.Create')">
            <div>
                <Form ref="newProductForm" label-position="top" :rules="newProductRule" :model="createProduct">
                    <Tabs value="detail">
                        <TabPane :label="$t('Public.Details')" name="detail">
                            <FormItem :label="$t('Products.ProductName')" prop="productName">
                                <Input v-model="createProduct.productName" :maxlength="200" :minlength="1"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.AbbreName')" prop="abbreName">
                                <Input v-model="createProduct.abbreName" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.ProductNo')" prop="productNo">
                                <Input v-model="createProduct.productNo" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.Sku')" prop="sku">
                                <Input v-model="createProduct.sku" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.TaxNo')" prop="taxNo">
                                <Input v-model="createProduct.taxNo" :maxlength="20"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.Brand')" prop="brand">
                                <Input v-model="createProduct.brand" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.Weight')" prop="weight">
                                <Input-number v-model.number="createProduct.weight" style="width:100%"></Input-number>
                            </FormItem>
                        </TabPane>
                    </Tabs>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="create" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
        <Modal v-model="showEditModal" :title="$t('Public.Edit')">
            <div>
                <Form ref="productForm" label-position="top" :rules="productRule" :model="editProduct">
                    <Tabs value="detail">
                        <TabPane :label="$t('Public.Details')" name="detail">
                            <FormItem :label="$t('Products.ProductName')" prop="productName">
                                <Input v-model="editProduct.productName" :maxlength="200" :minlength="1"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.AbbreName')" prop="abbreName">
                                <Input v-model="editProduct.abbreName" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.ProductNo')" prop="productNo">
                                <Input v-model="editProduct.productNo" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.Sku')" prop="sku">
                                <Input v-model="editProduct.sku" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.TaxNo')" prop="taxNo">
                                <Input v-model="editProduct.taxNo" :maxlength="20"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.Brand')" prop="brand">
                                <Input v-model="editProduct.brand" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="$t('Products.Weight')" prop="weight">
                                <Input-number v-model.number="editProduct.weight" style="width:100%"></Input-number>
                            </FormItem>
                            <FormItem>
                                <Checkbox v-model="editProduct.isActive">{{$t('Public.IsActive')}}</Checkbox>
                            </FormItem>
                        </TabPane>
                    </Tabs>                    
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showEditModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="edit" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script>
import ProductApi from '@/api/product'

export default {
    methods:{
        async create(){
            this.$refs.newProductForm.validate(async (val)=>{
                if(val){
                    await ProductApi.Create(this.createProduct);
                    this.showModal=false;
                    await this.getpage();
                }
            })
        },
        async edit(){
            this.$refs.productForm.validate(async (val)=>{
                if(val){
                    await ProductApi.Update(this.editProduct);
                    this.showEditModal=false;
                    await this.getpage();
                }
            })
        },
        pageChange(page){
            this.state.currentPage=page;
            this.getpage();
        },
        pagesizeChange(pagesize){
            this.state.pageSize=pagesize;
            this.getpage();
        },
        async getpage(){
            let page={
                maxResultCount:this.state.pageSize,
                skipCount:(this.state.currentPage-1)*this.state.pageSize
            }
            let rep= await ProductApi.Search({params:page});
            this.state.products=[];
            this.state.products.push(...rep.data.result.items);
            this.state.totalCount=rep.data.result.totalCount;
        },
        handleClickActionsDropdown(name){
            if(name==='Create'){
                this.showModal=true;
            }else if(name==='Refresh'){
                this.getpage();
            }
        }
    },
    data(){
        return{
            editProduct:{},
            createProduct:{},
            showModal:false,
            showEditModal:false,
            newProductRule:{
                productName:[{required: true,trigger: 'blur'}],
                productNo:[{required:true,trigger: 'blur'}],
                sku:[{required:true,trigger: 'blur'}],
                weight:[{type: 'number'}]
            },
            productRule:{
                productName:[{required: true,trigger: 'blur'}],
                productNo:[{required:true,trigger: 'blur'}],
                sku:[{required:true,trigger: 'blur'}],
                weight:[{type: 'number'}]
            },
            columns:[{
                title:this.$t('Products.ProductName'),
                key:'productName'
            },{
                title:this.$t('Products.AbbreName'),
                key:'abbreName'
            },{
                title:this.$t('Products.ProductNo'),
                key:'productNo'
            },{
                title:this.$t('Products.Sku'),
                key:'sku'
            },{
                title:this.$t('Products.TaxNo'),
                key:'taxNo'
            },{
                title:this.$t('Products.Brand'),
                key:'brand'
            },{
                title:this.$t('Products.Weight'),
                key:'weight'
            },{
                title:this.$t('Public.IsActive'),
                render:(h,params)=>{
                    return h('Checkbox',{
                        props:{
                            value:this.products[params.index].isActive,
                            disabled:true
                        }
                    })
                }
            },{
                title: this.$t('Public.Actions'),
                key: 'action',
                width:150,
                render:(h,params)=>{
                    return h('div',[
                        h('Button',{
                            props:{
                                type:'primary',
                                size:'small'
                            },
                            style:{
                                marginRight:'5px'
                            },
                            on:{
                                click:()=>{
                                    this.editProduct=this.products[params.index];
                                    this.showEditModal=true;
                                }
                            }
                        },this.$t('Public.Edit')),
                        h('Button',{
                            props:{
                                type:'error',
                                size:'small'
                            },
                            on:{
                                click:async()=>{
                                    this.$Modal.confirm({
                                        title:this.$t(''),
                                        content:this.$t('Products.Delete product'),
                                        okText:this.$t('Public.Yes'),
                                        cancelText:this.$t('Public.No'),
                                        onOk:async()=>{
                                            await ProductApi.Delete(this.products[params.index].id)
                                            await this.getpage();
                                        }
                                    })
                                }
                            }
                        },this.$t('Public.Delete'))
                    ])
                }
            }],
            state: {
                products:[],
                totalCount:0,
                pageSize:10,
                currentPage:1
            }
        }
    },
    computed:{
        products(){
            return this.state.products;
        },
        totalCount(){
            return this.state.totalCount;
        },
        currentPage(){
            return this.state.currentPage;
        },
        pageSize(){
            return this.state.pageSize;
        }
    },
    async created(){
        this.getpage();
    }
}
</script>