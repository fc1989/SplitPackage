<template>
    <div>
        <Card>
            <p slot="title">{{$t('Tenants')}}</p>
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
            <Table :columns="columns" border :data="tenants"></Table>
            <Page :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </Card>
        <Modal v-model="showModal" :title="$t('Public.Create')" @on-ok="save" :okText="$t('Public.Save')" :cancelText="$t('Public.Cancel')">
            <div>
                <Form ref="newTenantForm" label-position="top" :rules="newtenantRule" :show-message="false" :model="editTenant">
                    <FormItem :label="$t('Tenants.TenancyName')" prop="tenancyName">
                        <Input v-model="editTenant.tenancyName" :maxlength="64" :minlength="2"></Input>
                    </FormItem>
                    <FormItem :label="$t('Public.Name')" prop="name">
                        <Input v-model="editTenant.name" :maxlength="128"></Input>
                    </FormItem>
                    <FormItem :label="$t('Tenants.DatabaseConnectionString')+'('+$t('Public.Optional')+')'">
                        <Input v-model="editTenant.connectionString" :maxlength="1024"></Input>
                    </FormItem>
                    <FormItem :label="$t('Tenants.AdminEmailAddress')" prop="adminEmailAddress">
                        <Input v-model="editTenant.adminEmailAddress" type="email" :maxlength="256"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="editTenant.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                    <p><p>{{$t('Public.DefaultPasswordIs','123qwe')}}</p></p>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="save" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
        <Modal v-model="showEditModal" :title="$t('Public.Edit')" @on-ok="save" :okText="$t('Public.Save')" :cancelText="$t('Public.Cancel')">
            <div>
                <Form ref="tenantForm" label-position="top" :rules="tenantRule" :show-message="false" :model="editTenant">
                    <FormItem :label="$t('Tenants.TenancyName')" prop="tenancyName">
                        <Input v-model="editTenant.tenancyName" :maxlength="64" :minlength="2"></Input>
                    </FormItem>
                    <FormItem :label="$t('Public.Name')" prop="name">
                        <Input v-model="editTenant.name" :maxlength="128"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="editTenant.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                    <p><p>{{$t('Public.DefaultPasswordIs','123qwe')}}</p></p>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showEditModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="save" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script>
export default {
    methods:{
        create(){
            this.editTenant={isActive:true};
            this.showModal=true;
        },
        async save(){
            if(!!this.editTenant.id){
                this.$refs.tenantForm.validate(async (val)=>{
                    if(val){
                        await this.$store.dispatch({
                            type:'tenant/update',
                            data:this.editTenant
                        })
                        this.showEditModal=false;
                        await this.getpage();
                    }
                })
                
            }else{
                this.$refs.newTenantForm.validate(async (val)=>{
                    if(val){
                        await this.$store.dispatch({
                            type:'tenant/create',
                            data:this.editTenant
                        })
                        this.showModal=false;
                        await this.getpage();
                    }
                })
            }
            
        },
        pageChange(page){
            this.$store.commit('tenant/setCurrentPage',page);
            this.getpage();
        },
        pagesizeChange(pagesize){
            this.$store.commit('tenant/setPageSize',pagesize);
            this.getpage();
        },
        async getpage(){
            await this.$store.dispatch({
                type:'tenant/getAll'
            })
        },
        handleClickActionsDropdown(name){
            if(name==='Create'){
                this.create();
            }else if(name==='Refresh'){
                this.getpage();
            }
        }
    },
    data(){
        return{
            editTenant:{},
            showModal:false,
            showEditModal:false,
            newtenantRule:{
                tenancyName:[{required: true}],
                name:[{required:true}],
                adminEmailAddress:[{required:true},{type: 'email'}]
            },
            tenantRule:{
                tenancyName:[{required: true}],
                name:[{required:true}],
            },
            columns:[{
                title:this.$t('Tenants.TenancyName'),
                key:'tenancyName'
            },{
                title:this.$t('Public.Name'),
                key:'name'
            },{
                title:this.$t('Public.IsActive'),
                render:(h,params)=>{
                    return h('Checkbox',{
                        props:{
                            value:this.tenants[params.index].isActive,
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
                                    this.editTenant=this.tenants[params.index];
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
                                        content:this.$t('Tenants.Delete tenant'),
                                        okText:this.$t('Public.Yes'),
                                        cancelText:this.$t('Public.No'),
                                        onOk:async()=>{
                                            await this.$store.dispatch({
                                                type:'tenant/delete',
                                                data:this.tenants[params.index]
                                            })
                                            await this.getpage();
                                        }
                                    })
                                }
                            }
                        },this.$t('Public.Delete'))
                    ])
                }
            }]
        }
    },
    computed:{
        tenants(){
            return this.$store.state.tenant.tenants;
        },
        totalCount(){
            return this.$store.state.tenant.totalCount;
        },
        currentPage(){
            return this.$store.state.tenant.currentPage;
        },
        pageSize(){
            return this.$store.state.tenant.pageSize;
        }
    },
    created(){
        this.getpage();
    }
}
</script>

