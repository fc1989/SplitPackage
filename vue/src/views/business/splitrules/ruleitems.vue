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
                    <FormItem :label="$t('Menu.Pages.ProductClasses')" prop="productClassId">
                        <Cascader :data="cascaderData" v-model="cascaderValue"></Cascader>
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

<script>
import ProductClassApi from "@/api/productclass";
import SplitRuleItemApi from "@/api/splitruleitem";

const addHeaderRender = (h, param, vm, showAddIcon, clickAction) => {
    var array = [];
    if(showAddIcon){
        array.push(h("Icon", { props: {type:"android-add-circle",color:"#57a3f3"}}));
    }
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
                for(var item in vm.cascaderData){
                    var array1 = vm.cascaderData[item].children.filter(vl => {
                        return vl.value === ptid;
                    });
                    if(array1.length > 0)
                    {
                        productSortId = vm.cascaderData[item].value;
                        break;          
                    }
                }
                vm.cascaderValue = [productSortId,ptid];
                vm.modalState.actionState = "edit";
                vm.modalState.model = params.row;
                vm.modalState.showModal = true;
                vm.modalState.title = vm.$t('Public.Edit') + vm.$t('Menu.Pages.LogisticChannels');
            }
        }
    },vm.$t('Public.Edit')));
    array.push(h("Button",
    {
        props: {
            type: "info",
            size: "small"
        },
        style: {
            marginRight: "5px"
        },
        on: {
            click: () => {
                SplitRuleItemApi.Get(params.row.id).then(req=>{
                    vm.modalState.actionState = "detail";
                    vm.modalState.model = req.data.result;
                    vm.modalState.showModal = true;
                    vm.modalState.title = vm.$t('Menu.Pages.LogisticChannels')+vm.$t('Public.Details');
                });
            }
        }
    },vm.$t('Public.Details')));
    return h("div", array);
};

export default {
    props: {
        splitRuleId: Number,
        splitRuleName: String
    },
    data() {
        var _this = this;
        const validateProductClass = (rule, value, callback) => {
            if (_this.cascaderValue.length < 1) {
                callback(new Error("ProductClass is required"));
            }
            callback();
        };
        return {
            columns: [
                {
                    title: this.$t('SplitRules.RuleName'),
                    renderHeader: (h, params) => { 
                        return addHeaderRender(h, params, _this, !_this.isImport,function(vm){
                            vm.cascaderValue = [];
                            vm.modalState.model = {
                                maxNum: 0,
                                minNum: 0
                            };
                            vm.modalState.showModal = true;
                            vm.modalState.actionState = "create";
                            vm.modalState.title = vm.$t('Public.Create') + vm.$t('Menu.Pages.LogisticChannels');
                        }); 
                    },
                    render: (h)=>{
                        return h('span',this.splitRuleName);
                    }
                },
                {
                    title: _this.$t("Menu.Pages.ProductClasses"),
                    align: "center",
                    key: "ptid",
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
                    productClassId: [{required: true, validator: validateProductClass}],
                    maxNum: [{required: true,}],
                    minNum: [{required: true,}],
                },
                title: null,
                showModal: false,
                actionState: null,
            },
            cascaderData: [],
            cascaderValue: []
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
                    this.modalState.model.ptid = this.cascaderValue[1];
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
