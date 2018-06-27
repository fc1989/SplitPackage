<template>
    <div>
        <div>
            <Table :columns="columns" :data="tableData" border disabled-hover></Table>
            <Page :total="totalCount" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </div>
        <Modal v-model="modalState.showModal" :title="modalState.title">
            <div>
                <Form ref="modalForm" :rules="modalState.rule" :model="modalState.model" :label-width="70">
                    <FormItem :label="$t('SplitRules.RuleName')">
                        {{splitRuleName}}
                    </FormItem>
                    <FormItem :label="$t('SplitRules.Type')" prop="way" :labelWidth="70">
                        <Select v-model="modalState.model.type">
                            <Option v-for="(option,key) in modalState.stintType" :value="Number.parseInt(key)" :key="key">{{option}}</Option>
                        </Select>
                    </FormItem>
                    <FormItem v-if="modalState.model.type === 0" :label="$t('Menu.Pages.ProductClasses')" prop="cascaderValue">
                        <Cascader :data="cascaderData" v-model="modalState.model.cascaderValue"></Cascader>
                    </FormItem>
                    <FormItem v-else :label="'Sku'" prop="sku">
                        <Select
                            v-model="modalState.model.sku"
                            filterable
                            remote
                            :remote-method="skuRemoteMethod"
                            :loading="modalState.skuLoading">
                            <Option v-for="option in modalState.skuOptions" :value="option.value" :key="option.value">{{option.label}}</Option>
                        </Select>
                    </FormItem>
                    <FormItem :label="$t('SplitRules.MaxNum')" prop="maxNum">
                        <InputNumber v-model.number="modalState.model.maxNum"></InputNumber>
                    </FormItem>
                    <FormItem :label="$t('SplitRules.MinNum')" prop="minNum">
                        <InputNumber v-model.number="modalState.model.minNum"></InputNumber>
                    </FormItem>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="modalState.showModal=false">{{$t('Public.Cancel')}}</Button>
                <Button v-if="modalState.actionState!='detail'" @click="modalMethod" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
    </div>
</template>

<style>
    b {
        font-family: cursive;
    }
</style>

<script>
import ProductClassApi from "@/api/productclass";
import SplitRuleItemApi from "@/api/splitruleitem";
import ProductApi from "@/api/product";

const addHeaderRender = (h, param, vm, clickAction) => {
    var array = [];
    array.push(h("Icon",           {
        style: {
            "font-Size": "14px",
            "padding-right":"10px"
        },
        props: {
            type:"plus"
        }
    }));
    array.push(h("span", param.column.title));
    return h("div",
        {
            on: {
                click: async () =>{
                    clickAction(vm);
                }
            }
        },
        array
    );
};
const rowActionRender = (h, params, vm) => {
    var array = [];
    if(params.row.tenantId === vm.$store.state.session.tenantId){
        array.push(h("Button",
        {
            props: {
                type: "primary",
                size: "small"
            },
            style: {
                marginRight: "5px"
            },
            on: {
                click: () => {
                    var productSortId = "";
                    var ptid = params.row.ptid;
                    vm.modalState.actionState = "edit";
                    vm.modalState.model = params.row;
                    if(params.row.type == 0)
                    {
                        for(var item in vm.cascaderData){
                            var array1 = vm.cascaderData[item].children.filter(vl => {
                                return vl.value === ptid;
                            });
                            if(array1.length > 0){
                                productSortId = vm.cascaderData[item].value;
                                break;
                            }
                        }
                        vm.modalState.model = $.extend({},vm.modalState.model,{cascaderValue:[productSortId,ptid]});
                    }
                    vm.modalState.showModal = true;
                    vm.modalState.title = vm.$t('Public.Edit') + vm.$t('Menu.Pages.LogisticChannels');
                }
            }
        },vm.$t('Public.Edit')));
        array.push(h("Button",{
            props: {
                type: "error",
                size: "small"
            },
            on: {
                click: async () => {
                    vm.$Modal.confirm({
                    title: vm.$t(''),
                    content: vm.$t('Public.Delete') + vm.$t(vm.title),
                    okText: vm.$t('Public.Yes'),
                    cancelText: vm.$t('Public.No'),
                    onOk: () => {
                        SplitRuleItemApi.Delete(params.row.id).then(req => {
                            vm.getpage();
                        });
                    }});
                }
            }
        },
        vm.$t('Public.Delete')));
    }
    return h("div", array);
};

export default {
    props: {
        splitRuleId: Number,
        splitRuleName: String,
        tenantId: Number
    },
    data() {
        var _this = this;
        return {
            columns: [
                {
                    title: this.$t('SplitRules.RuleName'),
                    renderHeader: (h, params) => { 
                        return addHeaderRender(h, params, _this, function(vm){
                            vm.modalState.model = {
                                maxNum: 0,
                                minNum: 0,
                                type: 0,
                                cascaderValue:[]
                            };
                            vm.modalState.showModal = true;
                            vm.modalState.actionState = "create";
                            vm.modalState.title = vm.$t('Public.Create') + vm.$t('Menu.Pages.RuleItems');
                        }); 
                    },
                    render: (h)=>{
                        return h('span',this.splitRuleName);
                    }
                },
                {
                    renderHeader: (h, params) => {
                        var array = [h("span",_this.$t("Menu.Pages.ProductClasses")),h("b","/"),h("span","sku")];
                        return h('span',array);
                    },
                    align: "center",
                    key: "productClass",
                },
                {
                    title: _this.$t("SplitRules.MaxNum"),
                    align: "center",
                    key: "maxNum",
                },
                {
                    title: _this.$t("SplitRules.MinNum"),
                    align: "center",
                    key: "minNum",
                },
                {
                    title: _this.$t("Public.Actions"),
                    align: "center",
                    width: 190,
                    key: "action",
                    render: (h, param) => { return rowActionRender(h, param, _this);}
                }
            ],
            state: {
                tableData: [],
                totalCount: 0,
                pageSize: 5,
                currentPage: 1
            },
            modalState: {
                model: {},
                rule: {
                    cascaderValue: [{required: true, trigger: 'ignore'}],
                    sku:[{required: true, trigger: 'ignore'}],
                    maxNum: [{required: true, type:'number', trigger: 'ignore'}],
                    minNum: [{required: true, type:'number', trigger: 'ignore'}],
                },
                title: null,
                showModal: false,
                actionState: null,
                stintType: this.$store.state.app.enumInformation.ruleItemStintType,
                skuOptions: [],
                skuLoading: false
            },
            cascaderData: []
        };
    },
    methods: {
        pageChange(page) {
            this.state.currentPage = page;
            this.getpage();
        },
        pagesizeChange(pagesize) {
            this.state.pageSize = pagesize;
            this.getpage();
        },
        async getpage() {
            let page = {
                maxResultCount: this.state.pageSize,
                skipCount: (this.state.currentPage - 1) * this.state.pageSize,
                splitRuleId: this.splitRuleId
            };
            let rep = await SplitRuleItemApi.Search({ params: page });
            this.state.tableData = [];
            this.state.tableData.push(...rep.data.result.items);
            this.state.totalCount = rep.data.result.totalCount;
        },
        modalMethod(){
            this.$refs.modalForm.validate(async val => {
                if (val) {
                    if(this.modalState.model.type === 0){
                        this.modalState.model.ptid = this.modalState.model.cascaderValue[1];
                    }
                    else if(this.modalState.model.type === 1){
                        this.modalState.model.ptid = this.modalState.model.sku;
                    }
                    this.modalState.model.splitRuleId = this.splitRuleId;
                    if(this.modalState.model.id){
                        await SplitRuleItemApi.Update(this.modalState.model);
                    }
                    else{
                        await SplitRuleItemApi.Create(this.modalState.model);
                    }
                    this.modalState.showModal = false;
                    this.getpage();
                }
            });
        },
        skuRemoteMethod(query){
            let _this = this;
            if (query !== '') {
                this.modalState.skuLoading = true;
                ProductApi.GetOwnOption(query).then(req=>{
                    _this.modalState.skuLoading = false;
                    _this.modalState.skuOptions = req.data.result;
                });
            } else {
                _this.modalState.skuOptions = [];
            }
        }
    },
    computed: {
        tableData() {
            return this.state.tableData;
        },
        totalCount() {
            return this.state.totalCount;
        },
        currentPage() {
            return this.state.currentPage;
        },
        pageSize() {
            return this.state.pageSize;
        }
    },
    created() {
        var _this = this;
        this.getpage();
        ProductClassApi.GetOptional().then(req => {
            _this.cascaderData = req.data.result.map(function(vl, index, arr){
                return {
                    value: vl.value,
                    label: vl.label,
                    children: vl.children.map(function(vl1, index1, arr1){
                        return {
                            value: vl1.value,
                            label: vl1.label,
                        };
                    })
                };
            });
        });
    }
};
</script>
