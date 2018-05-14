<template>
    <div>
        <div>
            <Table :row-class-name="rowClass" :columns="columns" border :data="tableData"></Table>
            <Page :total="totalCount" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </div>
        <Modal v-model="modalState.showModal" :title="modalState.title" :width="modalWidth">
            <div>
                <Form ref="modalForm" :rules="modalState.rule" :model="modalState.model" :label-width="70">
                    <Row>
                        <Col span="12">
                            <FormItem :label="$t('Menu.Pages.Logistics')">
                                {{modalState.model.logisticName}}
                            </FormItem>
                        </Col>
                        <Col span="12">
                            <FormItem :label="$t('LogisticChannels.ChannelName')" prop="channelName">
                                <Input v-model="modalState.model.channelName" :maxlength="50" :minlength="1" :disabled="modalState.actionState != 'create'"></Input>
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span="12">
                            <FormItem :label="$t('LogisticChannels.Type')" prop="type">
                                <Select v-model="modalState.model.type" :disabled="modalState.actionState==='detail'">
                                    <Option v-for="(option,key) in modalState.channelType" :value="Number.parseInt(key)" :key="key">{{option}}</Option>
                                </Select>
                            </FormItem>
                        </Col>
                        <Col span="12">
                            <FormItem :label="$t('LogisticChannels.AliasName')" prop="aliasName">
                                <Input v-model="modalState.model.aliasName" :maxlength="50" :disabled="modalState.actionState==='detail'"></Input>
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span="12">
                            <FormItem :label="$t('Public.IsActive')" prop="isActive">
                                <Checkbox v-model="modalState.model.isActive" :disabled="modalState.actionState==='detail'"></Checkbox>
                            </FormItem>
                        </Col>
                        <Col span="12">
                            <FormItem :label="$t('LogisticChannels.Way')" prop="way">
                                <Select v-model="modalState.model.way" :disabled="modalState.actionState==='detail'">
                                    <Option v-for="(option,key) in modalState.chargeWay" :value="Number.parseInt(key)" :key="key">{{option}}</Option>
                                </Select>
                            </FormItem>
                        </Col>
                    </Row>
                    <Row v-if="modalState.model.way === 0">
                        <Col>
                            <label>{{$t('LogisticChannels.WeightChargeRule')}}</label>
                            <Table :columns="modalState.weightColumns" :data="modalState.model.weightChargeRules"></Table>
                        </Col>
                    </Row>
                    <Row v-if="modalState.model.way === 1">
                        <Col>
                            <label>{{$t('LogisticChannels.NumChargeRule')}}</label>
                            <Table :columns="modalState.numColumns" :data="modalState.model.numChargeRules"></Table>
                        </Col>
                    </Row>
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
    .ivu-table .logisticchannel-row td {
        background-color: #f8f8f9
    }
    .ivu-table .ivu-table-expanded-cell{
        background-color: #f8f8f9
    }
</style>

<script>
    import LogisticChannelApi from "@/api/logisticchannel";

    const addHeaderRender = (h, param, vm, clickAction) => {
        return h("div",
            {
                on: {
                    click: async () =>{
                        clickAction(vm);
                    }
                }
            },
            [h("Icon", { props: {type:"android-add-circle",color:"#57a3f3"}}),h("span", param.column.title)]
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
                        LogisticChannelApi.Get(params.row.id).then(req=>{
                            vm.modalState.model = req.data.result;
                            vm.modalState.showModal = true;
                            vm.modalState.actionState = "edit";
                            vm.modalState.title = vm.$t('Public.Edit') + vm.$t('Menu.Pages.LogisticChannels');
                        });
                    }
                }
            },vm.$t('Public.Edit')));
        }
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
                    LogisticChannelApi.Get(params.row.id).then(req=>{
                        vm.modalState.model = req.data.result;
                        vm.modalState.showModal = true;
                        vm.modalState.actionState = "detail";
                        vm.modalState.title = vm.$t('Menu.Pages.LogisticChannels')+vm.$t('Public.Details');
                    });
                }
            }
        },vm.$t('Public.Details')));
        return h("div", array);
    };
    const generaladdHeaderRender = (h, param, vm, clickAction) => {
        if(vm.modalState.actionState === 'detail'){
          return h('span',param.column.title)  
        }
        else{
            return addHeaderRender(h, param, vm, clickAction);
        }
    };
    const generalRender = (h, params, vm, isNumber) => {
        if(vm.modalState.actionState === 'detail'){
          return h('span',params.row[params.column.key])  
        }
        if(isNumber){
            return h("Input-number", {
                props: {
                    type: "number",
                    value: params.row[params.column.key]
                }
            });
        }else{
            return h("Input", {
                props: {
                    type: "text",
                    value: params.row[params.column.key]
                }
            });
        }
    };

    export default {
        props: {
            logisticId: Number,
            logisticName: String
        },
        data() {
            let _this = this;
            const validateChannelName = (rule, value, callback) => {
                if (!value) {
                    callback(new Error("channelName is required"));
                } else {
                    if(_this.modalState.actionState === "edit"){
                        callback();
                    }
                    LogisticChannelApi.Verify({logisticId:_this.logisticId,channelName:value}).then(function(rep) {
                        if (rep.data.result) {
                            callback();
                        } else {
                            callback("channelName is exit");
                        }
                    });
                }
            };
            const validateChargeRule = (rule, value, callback) =>{
                if(_this.modalState.model.way === 0 && (value || value.length ===0)){
                    callback("weightChargeRules is required");
                }
                if(_this.modalState.model.way === 1 && (value || value.length ===0)){
                    callback("numChargeRules is required");
                }
                callback();
            };
            return  {
                modalWidth: 750,
                columns: [{
                    title: this.$t('LogisticChannels.ChannelName'),
                    key: "channelName",
                    renderHeader: (h, param) => { return addHeaderRender(h, param, _this, function(vm){
                        vm.modalState.model = {
                            logisticId: vm.logisticId,
                            logisticName: vm.logisticName,
                            weightChargeRules: [],
                            numChargeRules: [],
                            type: 0,
                            way: 0
                        };
                        vm.modalState.showModal = true;
                        vm.modalState.actionState = "create";
                        vm.modalState.title = vm.$t('Public.Create') + vm.$t('Menu.Pages.LogisticChannels');
                    }); }
                },
                {
                    title: this.$t('LogisticChannels.Type'),
                    key: 'type',
                    render: (h,params) => {
                        return h("span", _this.modalState.channelType[params.row.type])
                    }
                },
                {
                    title: this.$t('LogisticChannels.Way'),
                    key: "way",
                    render: (h,params) => {
                        return h("span", _this.modalState.chargeWay[params.row.way]);
                    }
                },
                {
                    title: this.$t('Public.IsActive'),
                    render: (h, params) => {
                        return h("Checkbox", {
                            props: {
                                value: params.row.isActive,
                                disabled: true
                            }
                        });
                    }
                },
                {
                    title: this.$t("Public.Actions"),
                    type: 'action',
                    render: (h,params) => rowActionRender(h, params, _this)
                }],
                state: {
                    tableData: [],
                    totalCount: 0,
                    pageSize: 5,
                    currentPage: 1
                },
                modalState: {
                    model: {},
                    rule: {
                        channelName: [{ required: true, validator: validateChannelName }],
                        numChargeRules: [{validator: validateChargeRule}],
                        weightChargeRules: [{validator: validateChargeRule}]
                    },
                    title: null,
                    showModal: false,
                    actionState: null,
                    weightColumns: [{
                            title: this.$t('Public.Currency'),
                            key: "currency",
                            renderHeader: (h, params) => { return generaladdHeaderRender(h, params, _this, function(vm){
                                if(vm.modalState.model.weightChargeRules&&vm.modalState.model.weightChargeRules.length > 0){
                                    return;
                                }
                                vm.modalState.model.weightChargeRules.push({});
                            }); },
                            render: (h, params) => generalRender(h, params, _this)
                        },
                        {
                            title: this.$t('Public.Unit'),
                            key: "unit",
                            render: (h, params) => generalRender(h, params, _this)
                        },
                        {
                            title: this.$t('WeightFreights.StartingWeight'),
                            key: 'startingWeight',
                            render: (h, params) => generalRender(h, params, _this, true)
                        },
                        {
                            title: this.$t('WeightFreights.EndWeight'),
                            key: 'endWeight',
                            render: (h, params) => generalRender(h, params, _this, true)
                        },
                        {
                            title: this.$t('WeightFreights.StartingPrice'),
                            key: 'startingPrice',
                            render: (h, params) => generalRender(h, params, _this, true)
                        },
                        {
                            title: this.$t('WeightFreights.StepWeight'),
                            key: 'stepWeight',
                            render: (h, params) => generalRender(h, params, _this, true)
                        },
                        {
                            title: this.$t('WeightFreights.CostPrice'),
                            key: 'costPrice',
                            render: (h, params) => generalRender(h, params, _this, true)
                        },
                        {
                            title: this.$t('WeightFreights.Price'),
                            key: 'price',
                            render: (h, params) => generalRender(h, params, _this, true)
                        }
                    ],
                    numColumns: [{
                            title: this.$t('Public.Currency'),
                            key: "currency",
                            renderHeader: (h,param) => { return generaladdHeaderRender(h, param, _this, function(vm){
                                if(vm.modalState.model.numChargeRules&&vm.modalState.model.numChargeRules.length > 0){
                                    return;
                                }
                                vm.modalState.model.numChargeRules.push({});
                            }); },
                            render: (h,params) => generalRender(h, params, _this)
                        },
                        {
                            title: this.$t('Public.Unit'),
                            key: "unit",
                            render: (h, params) => generalRender(h, params, _this)
                        },
                        {
                            title: this.$t('NumFreights.SplitNum'),
                            key: 'splitNum',
                            render: (h, params) => generalRender(h, params, _this, true)
                        },
                        {
                            title: this.$t('NumFreights.FirstPrice'),
                            key: 'firstPrice',
                            render: (h, params) => generalRender(h, params, _this, true)
                        },
                        {
                            title: this.$t('NumFreights.CarryOnPrice'),
                            key: 'carryOnPrice',
                            render: (h, params) => generalRender(h, params, _this, true)
                        }
                    ],
                    channelType: this.$store.state.app.enumInformation.channelType,
                    chargeWay: this.$store.state.app.enumInformation.chargeWay
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
        methods: {
            rowClass(row, index) {
                return 'logisticchannel-row';
            },
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
                    logisticId: this.logisticId
                };
                let rep = await LogisticChannelApi.Search({ params: page });
                this.state.tableData = [];
                this.state.tableData.push(...rep.data.result.items);
                this.state.totalCount = rep.data.result.totalCount;
            },
            modalMethod(){
                this.$refs.modalForm.validate(async val => {
                    if (val) {
                        if(_this.modalState.model.id){
                            await LogisticChannelApi.Update(_this.modalState.model);
                        }
                        else{
                            await LogisticChannelApi.Create(_this.modalState.model);
                        }
                        _this.modalState.showModal = false;
                        _this.getpage();
                    }
                });
            }
        },
        async created() {
            this.getpage();
        }
    };
</script>