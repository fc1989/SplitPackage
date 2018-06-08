<template>
    <Card>
        <p slot="title">{{$t('Menu.Pages.Simulator')}}</p>
        <Dropdown slot="extra"  @on-click="handleClickActionsDropdown">
            <a href="javascript:void(0)">
                {{$t('Public.Actions')}}
                <Icon type="android-more-vertical"></Icon>
            </a>
            <DropdownMenu slot="list">
                <DropdownItem name='Clear'>{{$t('Public.Clear')}}</DropdownItem>
                <DropdownItem name='Create'>{{$t('Public.Create')}}</DropdownItem>
            </DropdownMenu>
        </Dropdown>
        <Row>
            <Col span="12">
                <Card>
                    <p slot="title">{{$t('Simulator.OrderSettings')}}</p>
                    <Form ref="splitRequestForm" :model="splitRequest" :rules="splitRequestRule" label-position="left" :label-width="70">
                        <Row>
                            <FormItem :label="$t('Simulator.SplitType')" prop="splitType">
                                <Select v-model="splitRequest.splitType">
                                    <Option value="0">{{$t('Simulator.General')}}</Option>
                                    <Option value="1">{{$t('Simulator.DesignatedLogistics')}}</Option>
                                </Select>
                            </FormItem>
                        </Row>
                        <Row>
                            <FormItem v-if="splitRequest.splitType == '0'" prop="type" :label="$t('Simulator.SplitPrinciple')" :key="0">
                                <Select v-model="splitRequest.type">
                                    <Option value="1" key="1">{{$t('Simulator.PriceFirst')}}</Option>
                                    <Option value="2" key="2">{{$t('Simulator.SpeedFirst')}}</Option>
                                    <Option value="3" key="3">{{$t('Simulator.QuanlityFirst')}}</Option>
                                </Select>
                            </FormItem>
                            <FormItem v-else-if="splitRequest.splitType == '1'" prop="logistics" :label="$t('Menu.Pages.Logistics')" :key="1">
                                <Select v-model="splitRequest.logistics" multiple filterable>
                                    <Option v-for="item in logistics" :value="item.key" :key="item.key">{{item.label}}</Option>
                                </Select>
                            </FormItem>
                        </Row>
                        <Row>
                            <FormItem :label="$t('Simulator.ProductList')"  prop="proList">
                                <Table ref="prolist" :columns="productColumns" :data="splitRequest.proList" border disabled-hover></Table>
                            </FormItem>
                        </Row>
                        <Row>
                            <Button @click="splitPackage" type="primary" icon="success">{{$t('Simulator.SplitPackage')}}</Button>
                        </Row>
                    </Form>
                    <Modal v-model="modalState.showModal" :title="modalState.title">
                        <div>
                            <Form ref="modalForm" :rules="modalState.rule" :model="modalState.model" :label-width="70">
                                <FormItem :label="$t('Simulator.ProNo')" prop="proNo">
                                    <Input v-model="modalState.model.proNo"></Input>
                                </FormItem>
                                <FormItem :label="$t('Simulator.ProName')" prop="proName">
                                    <Input v-model="modalState.model.proName"></Input>
                                </FormItem>
                                <FormItem :label="'SkuNo'" prop="skuNo">
                                    <Input v-model="modalState.model.skuNo"></Input>
                                </FormItem>
                                <FormItem :label="$t('Menu.Pages.ProductClasses')" prop="cascaderValue">
                                    <Cascader :data="cascaderData" v-model="modalState.model.cascaderValue"  @on-change="cascaderChange"></Cascader>
                                </FormItem>
                                <FormItem :label="$t('Simulator.Quantity')" prop="quantity">
                                    <InputNumber v-model.number="modalState.model.quantity"></InputNumber>
                                </FormItem>
                                <FormItem :label="$t('Simulator.ProPrice')" prop="proPrice">
                                    <InputNumber v-model.number="modalState.model.proPrice"></InputNumber>
                                </FormItem>
                                <FormItem :label="$t('Simulator.Weight')" prop="weight">
                                    <InputNumber v-model.number="modalState.model.weight"></InputNumber>
                                </FormItem>
                            </Form>
                        </div>
                        <div slot="footer">
                            <Button @click="modalState.showModal=false">{{$t('Public.Cancel')}}</Button>
                            <Button @click="modalMethod" type="primary">{{$t('Public.Save')}}</Button>
                        </div>
                    </Modal>
                </Card>
            </Col>
            <Col span="12">
                <Card>
                    <p slot="title">{{$t('Simulator.SplitResult')}}</p>
                    <Tabs>
                        <TabPane v-for="order in splitResult" :label="order.id" :key="order.id">
                            <Form>
                                <Row>
                                    <Col span="12">
                                        <FormItem :label="$t('Simulator.LogisticsCost')">
                                            {{order.logisticsCost + "AUD"}}
                                        </FormItem>
                                    </Col>
                                    <Col span="12">
                                        <FormItem :label="$t('Menu.Pages.Logistics')">
                                            {{order.logisticsName}}
                                        </FormItem>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col span="12">
                                        <FormItem :label="$t('Simulator.LogisticsUnitPrice')">
                                            {{order.logisticsUnitPrice + "AUD"}}
                                        </FormItem>
                                    </Col>
                                    <Col span="12">
                                        <FormItem :label="$t('Menu.Pages.LogisticChannels')">
                                            {{order.subBusinessName}}
                                        </FormItem>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col span="12">
                                        <FormItem :label="$t('Simulator.TaxCost')">
                                            {{order.taxCost + "AUD"}}
                                        </FormItem>
                                    </Col>
                                    <Col span="12">
                                        <FormItem :label="$t('Simulator.TotalPrice')">
                                            {{order.totalPrice + "AUD"}}
                                        </FormItem>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col span="12">
                                        <FormItem :label="$t('Simulator.TotalWeight')">
                                            {{order.totalWeight + "g"}}
                                        </FormItem>
                                    </Col>
                                    <Col span="12">
                                        <FormItem :label="$t('Simulator.Url')">
                                            {{order.url}}
                                        </FormItem>
                                    </Col>
                                </Row>
                                <Row>
                                    <Table ref="proList" :columns="splitProductColumns" border :data="order.proList"></Table>
                                </Row>
                            </Form>
                        </TabPane>
                    </Tabs>
                </Card>
            </Col>
        </Row>
    </Card>
</template>

<script>
import LogisticApi from "@/api/logistic";
import ProductClassApi from "@/api/productclass";
import SplitPackageApi from "@/api/splitpackage";

const addHeaderRender = (h, params, vm) => {
    return h("div",
        {
            on: {
                click: async () => {
                    vm.modalState.title = vm.$t('Public.Add') + vm.$t('Menu.Pages.Products');
                    vm.modalState.actionState = "create";
                    vm.modalState.model = {
                        quantity: 0,
                        proPrice: 0,
                        weight: 0
                    };
                    vm.modalState.showModal = true;
                }
            }
        },
        [h("Icon",
        {
            style: {
                "font-Size": "14px",
                "padding-right":"10px"
            },
            props: {
                type:"plus"
            }
        }),
        h("strong", params.column.title)]
    );
};
const actionRender = (h, params, vm) => {
    return h("div", [
        h("Button",
            {
                props: {
                    type: "primary",
                    size: "small"
                },
                style: {
                    margin: "0 5px"
                },
                on: {
                    click: () => {
                        vm.modalState.title = vm.$t('Public.Edit') + vm.$t('Menu.Pages.Products');
                        vm.modalState.actionState = "edit";
                        vm.modalState.model = params.row;
                        vm.modalState.showModal = true;
                    }
                }
            },
            params.row.editting ? vm.$t("Public.Save"): vm.$t("Public.Edit")
        ),
        h("Poptip",
            {
                props: {
                    confirm: true,
                    title: vm.$t('SplitRules.DeleteTip'),
                    transfer: true
                },
                on: {
                    "on-ok": () => {
                        vm.splitRequest.proList.splice(params.index, 1);
                        vm.$emit("input", vm.proList);
                    }
                }
            },
            [
                h("Button",
                    {
                        style: {
                            margin: "0 5px"
                        },
                        props: {
                            type: "error",
                            placement: "top",
                            size: "small"
                        }
                    },
                    vm.$t("Public.Delete")
                )
            ]
        )
    ]);
};

export default {
    data(){
        var _this = this;
        return {
            splitRequest: {
                orderId: '',
                splitType: '0',
                type: '1',
                logistics: [],
                proList: []
            },
            splitRequestRule:{
                logistics: [{required: true, type: 'array', trigger: 'ignore' }],
                proList: [{required: true, type:'array', trigger: 'ignore' }]
            },
            splitResult: [],
            logistics: [],
            productColumns: [
                {
                    title: this.$t("Simulator.ProNo"),
                    key: "proNo",
                    width: 90,
                    renderHeader: (h,params) => { return addHeaderRender(h, params, _this);}
                },
                {
                    title: this.$t("Simulator.ProName"),
                    width: 90,
                    key: "proName"
                },
                {
                    title: 'SkuNo',
                    width: 90,
                    key: "skuNo"
                },
                {
                    title: _this.$t('Menu.Pages.ProductClasses'),
                    width: 90,
                    key: "ptidName"
                },
                {
                    title: this.$t("Simulator.Quantity"),
                    width: 70,
                    key: "quantity"
                },
                {
                    title: this.$t("Simulator.ProPrice") + "(AUD)",
                    width: 95,
                    key: "proPrice"
                },
                {
                    title: this.$t("Simulator.Weight") + "(g)",
                    width: 110,
                    key: "weight"
                },
                {
                    title: _this.$t("Public.Actions"),
                    key: "action",
                    render: (h, params) => { return actionRender(h, params, _this);}
                }
            ],
            splitProductColumns: [
                {
                    title: this.$t("Simulator.ProNo"),
                    key: "proNo"
                },
                {
                    title: this.$t("Simulator.ProName"),
                    key: "proName"
                },
                {
                    title: 'SkuNo',
                    key: "skuNo"
                },
                {
                    title: 'PTId',
                    key: "ptid"
                },
                {
                    title: this.$t("Simulator.Quantity"),
                    key: "quantity"
                },
                {
                    title: this.$t("Simulator.ProPrice") + "(AUD)",
                    key: "proPrice"
                },
                {
                    title: this.$t("Simulator.Weight") + "(g)",
                    key: "weight"
                }
            ],
            modalState: {
                model: {},
                rule: {
                    cascaderValue:[{required: true, type: 'array', trigger: 'ignore'}],
                    quantity:[{required: true, type: 'number', min: 1, trigger: 'ignore'}],
                    proPrice:[{required: true, type: 'number', min: 1, trigger: 'ignore'}],
                    weight:[{required: true, type: 'number', min:1, trigger: 'ignore'}]
                },
                title: null,
                showModal: false,
                actionState: null,
            },
            cascaderData: []
        };
    },
    methods: {
        async handleClickActionsDropdown(name) {
            if (name === "Create") {

            } else if (name === "Clear") {

            }
        },
        cascaderChange(value, selectedData){
            this.modalState.model.ptid = value[1];
            this.modalState.model.ptidName = selectedData[1].label + "("+ value[1] +")";
        },
        modalMethod(){
            this.$refs.modalForm.validate(async val => {
                if (val) {
                    if(this.modalState.actionState === "create"){
                        this.splitRequest.proList.push(this.modalState.model);
                    }
                    else{
                        this.splitRequest.proList = JSON.parse(JSON.stringify(this.$refs.prolist.rebuildData));
                    }
                    this.modalState.showModal = false;
                }
            });
        },
        splitPackage(){
            var _this = this;
            this.$refs.splitRequestForm.validate(async val=>{
                if(val){
                    var remote = this.splitRequest.splitType === "0" ? SplitPackageApi.SplitOrder: SplitPackageApi.SplitOrderByAssign;
                    remote(this.splitRequest).then(req => {
                        if(req.data.code === 200){
                            _this.splitResult = req.data.result.orderList;
                        }
                        else{
                            _this.$Modal.error({
                                title: 'Error',
                                content: req.data.result
                            });
                        }
                    });
                }
            });
        }
    },
    async created(){
        var _this = this;
        LogisticApi.Search({
            params: {
                maxResultCount: 10000
            }
        }).then(req => {
            _this.logistics = req.data.result.items.map(function(vl, index, arr){
                return {
                    key: vl.logisticCode,
                    label: vl.corporationName
                };
            });
        });
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
}
</script>
