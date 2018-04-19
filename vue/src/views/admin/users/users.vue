<template>
    <div>
        <Card>
            <p slot="title">{{$t('Users')}}</p>
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
            <Table :columns="columns" border :data="users"></Table>
            <Page :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </Card>
        <Modal v-model="showModal" :title="$t('Public.Create')" @on-ok="save" :okText="$t('Public.Save')" :cancelText="$t('Public.Cancel')">
            <div>
                <Form ref="newUserForm" label-position="top" :rules="newUserRule" :model="editUser">
                    <Tabs value="detail">
                        <TabPane :label="$t('Public.Details')" name="detail">
                            <FormItem :label="$t('Public.UserName')" prop="userName">
                                <Input v-model="editUser.userName" :maxlength="32" :minlength="2"></Input>
                            </FormItem>
                            <FormItem :label="$t('Public.Name')" prop="name">
                                <Input v-model="editUser.name" :maxlength="32"></Input>
                            </FormItem>
                            <FormItem :label="$t('Users.Surname')" prop="surname">
                                <Input v-model="editUser.surname" :maxlength="1024"></Input>
                            </FormItem>
                            <FormItem :label="$t('Users.EmailAddress')" prop="emailAddress">
                                <Input v-model="editUser.emailAddress" type="email" :maxlength="32"></Input>
                            </FormItem>
                            <FormItem :label="$t('Users.Password')" prop="password">
                                <Input v-model="editUser.password" type="password" :maxlength="32"></Input>
                            </FormItem>
                            <FormItem :label="$t('Users.ConfirmPassword')" prop="confirmPassword">
                                <Input v-model="editUser.confirmPassword" type="password" :maxlength="32"></Input>
                            </FormItem>
                            <FormItem>
                                <Checkbox v-model="editUser.isActive">{{$t('Public.IsActive')}}</Checkbox>
                            </FormItem>
                        </TabPane>
                        <TabPane :label="$t('Users.UserRoles')" name="roles">
                            <CheckboxGroup v-model="editUser.roleNames">
                                <Checkbox :label="role.normalizedName" v-for="role in roles" :key="role.id"><span>{{role.name}}</span></Checkbox>
                            </CheckboxGroup>
                        </TabPane>
                    </Tabs>
                    
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="save" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
        <Modal v-model="showEditModal" :title="$t('Public.Edit')" @on-ok="save" :okText="$t('Public.Save')" :cancelText="$t('Public.Cancel')">
            <div>
                <Form ref="userForm" label-position="top" :rules="userRule" :model="editUser">
                    <Tabs value="detail">
                        <TabPane :label="$t('Public.Details')" name="detail">
                            <FormItem :label="$t('Public.UserName')" prop="userName">
                                <Input v-model="editUser.userName" :maxlength="32" :minlength="2"></Input>
                            </FormItem>
                            <FormItem :label="$t('Public.Name')" prop="name">
                                <Input v-model="editUser.name" :maxlength="32"></Input>
                            </FormItem>
                            <FormItem :label="$t('Users.Surname')" prop="surname">
                                <Input v-model="editUser.surname" :maxlength="1024"></Input>
                            </FormItem>
                            <FormItem :label="$t('Users.EmailAddress')" prop="emailAddress">
                                <Input v-model="editUser.emailAddress" type="email" :maxlength="32"></Input>
                            </FormItem>
                            <FormItem>
                                <Checkbox v-model="editUser.isActive">{{$t('Public.IsActive')}}</Checkbox>
                            </FormItem>
                        </TabPane>
                        <TabPane :label="$t('Users.UserRoles')" name="roles">
                            <CheckboxGroup v-model="editUser.roleNames">
                                <Checkbox :label="role.normalizedName" v-for="role in roles" :key="role.id"><span>{{role.name}}</span></Checkbox>
                            </CheckboxGroup>
                        </TabPane>
                    </Tabs>                    
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
            this.editUser={isActive:true};
            this.showModal=true;
        },
        async save(){
            if(!!this.editUser.id){
                this.$refs.userForm.validate(async (val)=>{
                    if(val){
                        await this.$store.dispatch({
                            type:'user/update',
                            data:this.editUser
                        })
                        this.showEditModal=false;
                        await this.getpage();
                    }
                })
                
            }else{
                this.$refs.newUserForm.validate(async (val)=>{
                    if(val){
                        await this.$store.dispatch({
                            type:'user/create',
                            data:this.editUser
                        })
                        this.showModal=false;
                        await this.getpage();
                    }
                })
            }
            
        },
        pageChange(page){
            this.$store.commit('user/setCurrentPage',page);
            this.getpage();
        },
        pagesizeChange(pagesize){
            this.$store.commit('user/setPageSize',pagesize);
            this.getpage();
        },
        async getpage(){
            await this.$store.dispatch({
                type:'user/getAll'
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
        const validatePassCheck = (rule, value, callback) => {
            if (!value) {
                callback(new Error('Please enter your password again'));
            } else if (value !== this.editUser.password) {
                callback(new Error('The two input passwords do not match!'));
            } else {
                callback();
            }
        };
        return{
            editUser:{},
            showModal:false,
            showEditModal:false,
            newUserRule:{
                userName:[{required: true,trigger: 'blur'}],
                name:[{required: true,trigger: 'blur'}],
                surname:[{required: true,trigger: 'blur'}],
                emailAddress:[{required: true,trigger: 'blur'},{type: 'email'}],
                password:[{required: true,trigger: 'blur'}],
                confirmPassword:{validator:validatePassCheck,trigger: 'blur'}
            },
            
            userRule:{
                userName:[{required: true,trigger: 'blur'}],
                name:[{required: true,trigger: 'blur'}],
                surname:[{required: true,trigger: 'blur'}],
                emailAddress:[{required: true,trigger: 'blur'},{type: 'email'}],
            },
            columns:[{
                title:this.$t('Public.UserName'),
                key:'userName'
            },{
                title:this.$t('Users.FullName'),
                key:'fullName'
            },{
                title:this.$t('Users.EmailAddress'),
                key:'emailAddress'
            },{
                title:this.$t('Public.IsActive'),
                render:(h,params)=>{
                    return h('Checkbox',{
                        props:{
                            value:this.users[params.index].isActive,
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
                                    this.editUser=this.users[params.index];
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
                                        content:this.$t('Users.Delete user'),
                                        okText:this.$t('Public.Yes'),
                                        cancelText:this.$t('Public.No'),
                                        onOk:async()=>{
                                            await this.$store.dispatch({
                                                type:'user/delete',
                                                data:this.users[params.index]
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
        users(){
            return this.$store.state.user.users;
        },
        roles(){
            return this.$store.state.user.roles;
        },
        totalCount(){
            return this.$store.state.user.totalCount;
        },
        currentPage(){
            return this.$store.state.user.currentPage;
        },
        pageSize(){
            return this.$store.state.user.pageSize;
        }
    },
    async created(){
        this.getpage();
        await this.$store.dispatch({
            type:'user/getRoles'
        });
    }
}
</script>


