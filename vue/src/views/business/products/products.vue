<template>
    <div>
        <Card>
            <p slot="title">{{'Products'|l}}</p>
            <Dropdown slot="extra"  @on-click="handleClickActionsDropdown">
                <a href="javascript:void(0)">
                    {{'Actions'|l}}
                    <Icon type="android-more-vertical"></Icon>
                </a>
                <DropdownMenu slot="list">
                    <DropdownItem name='Refresh'>{{'Refresh'|l}}</DropdownItem>
                    <DropdownItem name='Create'>{{'Create'|l}}</DropdownItem>
                </DropdownMenu>
            </Dropdown>
            <Table :columns="columns" border :data="products"></Table>
            <Page :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </Card>
        <Modal v-model="showModal" :title="L('CreateNewProduct')" @on-ok="save" :okText="L('save')" :cancelText="L('cancel')">
            <div>
                <Form ref="newProductForm" label-position="top" :rules="newProductRule" :model="createProduct">
                    <Tabs value="detail">
                        <TabPane :label="L('ProductDetails')" name="detail">
                            <FormItem :label="L('ProductName')" prop="productName">
                                <Input v-model="createProduct.productName" :maxlength="200" :minlength="1"></Input>
                            </FormItem>
                            <FormItem :label="L('AbbreName')" prop="abbreName">
                                <Input v-model="createProduct.abbreName" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('ProductNo')" prop="productNo">
                                <Input v-model="createProduct.productNo" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('Sku')" prop="sku">
                                <Input v-model="createProduct.sku" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('TaxNo')" prop="taxNo">
                                <Input v-model="createProduct.taxNo" :maxlength="20"></Input>
                            </FormItem>
                            <FormItem :label="L('Brand')" prop="brand">
                                <Input v-model="createProduct.brand" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('Weight')" prop="weight">
                                <Input v-model.number="createProduct.weight"></Input>
                            </FormItem>
                        </TabPane>
                    </Tabs>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showModal=false">{{'Cancel'|l}}</Button>
                <Button @click="save" type="primary">{{'Save'|l}}</Button>
            </div>
        </Modal>
        <Modal v-model="showEditModal" :title="L('EditProduct')" @on-ok="save" :okText="L('save')" :cancelText="L('cancel')">
            <div>
                <Form ref="productForm" label-position="top" :rules="productRule" :model="editProduct">
                    <Tabs value="detail">
                        <TabPane :label="L('ProductDetails')" name="detail">
                            <FormItem :label="L('ProductName')" prop="productName">
                                <Input v-model="editProduct.productName" :maxlength="200" :minlength="1"></Input>
                            </FormItem>
                            <FormItem :label="L('AbbreName')" prop="abbreName">
                                <Input v-model="editProduct.abbreName" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('ProductNo')" prop="productNo">
                                <Input v-model="editProduct.productNo" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('Sku')" prop="sku">
                                <Input v-model="editProduct.sku" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('TaxNo')" prop="taxNo">
                                <Input v-model="editProduct.taxNo" :maxlength="20"></Input>
                            </FormItem>
                            <FormItem :label="L('Brand')" prop="brand">
                                <Input v-model="editProduct.brand" :maxlength="50"></Input>
                            </FormItem>
                            <FormItem :label="L('Weight')" prop="weight">
                                <Input v-model.number="editProduct.weight"></Input>
                            </FormItem>
                            <FormItem>
                                <Checkbox v-model="editProduct.isActive">{{'IsActive'|l}}</Checkbox>
                            </FormItem>
                        </TabPane>
                    </Tabs>                    
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showEditModal=false">{{'Cancel'|l}}</Button>
                <Button @click="save" type="primary">{{'Save'|l}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script>
export default {
    methods:{
        async save(){
            if(!!this.editProduct.id){
                console.log(this.editProduct);
                this.$refs.productForm.validate(async (val)=>{
                    if(val){
                        await this.$store.dispatch({
                            type:'product/update',
                            data:this.editProduct
                        })
                        this.showEditModal=false;
                        await this.getpage();
                    }
                })
                
            }else{
                this.$refs.newProductForm.validate(async (val)=>{
                    if(val){
                        await this.$store.dispatch({
                            type:'product/create',
                            data:this.editProduct
                        })
                        this.showModal=false;
                        await this.getpage();
                    }
                })
            }
            
        },
        pageChange(page){
            this.$store.commit('product/setCurrentPage',page);
            this.getpage();
        },
        pagesizeChange(pagesize){
            this.$store.commit('product/setPageSize',pagesize);
            this.getpage();
        },
        async getpage(){
            await this.$store.dispatch({
                type:'product/getAll'
            })
        },
        handleClickActionsDropdown(name){
            if(name==='Create'){

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
                productName:[{required: true,message:'Product Name is required',trigger: 'blur'}],
                productNo:[{required:true,message:'ProductNo is required',trigger: 'blur'}],
                sku:[{required:true,message:'Sku is required',trigger: 'blur'}],
                weight:[{type: 'number'}]
            },
            productRule:{
                productName:[{required: true,message:'Product Name is required',trigger: 'blur'}],
                productNo:[{required:true,message:'ProductNo is required',trigger: 'blur'}],
                sku:[{required:true,message:'Sku is required',trigger: 'blur'}],
                weight:[{type: 'number'}]
            },
            columns:[{
                title:this.L('ProductName'),
                key:'productName'
            },{
                title:this.L('AbbreName'),
                key:'abbreName'
            },{
                title:this.L('ProductNo'),
                key:'productNo'
            },{
                title:this.L('Sku'),
                key:'sku'
            },{
                title:this.L('TaxNo'),
                key:'taxNo'
            },{
                title:this.L('Brand'),
                key:'brand'
            },{
                title:this.L('Weight'),
                key:'weight'
            },{
                title:this.L('IsActive'),
                render:(h,params)=>{
                    return h('Checkbox',{
                        props:{
                            value:this.products[params.index].isActive,
                            disabled:true
                        }
                    })
                }
            },{
                title: this.L('Actions'),
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
                        },this.L('Edit')),
                        h('Button',{
                            props:{
                                type:'error',
                                size:'small'
                            },
                            on:{
                                click:async()=>{
                                    this.$Modal.confirm({
                                        title:this.L(''),
                                        content:this.L('Delete product'),
                                        okText:this.L('Yes'),
                                        cancelText:this.L('No'),
                                        onOk:async()=>{
                                            await this.$store.dispatch({
                                                type:'product/delete',
                                                data:this.products[params.index]
                                            })
                                            await this.getpage();
                                        }
                                    })
                                }
                            }
                        },this.L('Delete'))
                    ])
                }
            }]
        }
    },
    computed:{
        products(){
            return this.$store.state.product.products;
        },
        totalCount(){
            return this.$store.state.product.totalCount;
        },
        currentPage(){
            return this.$store.state.product.currentPage;
        },
        pageSize(){
            return this.$store.state.product.pageSize;
        }
    },
    async created(){
        this.getpage();
    }
}
</script>