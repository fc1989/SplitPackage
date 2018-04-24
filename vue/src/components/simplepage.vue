<template>
    <div>
        <Card>
            <p slot="title">{{$t(title)}}</p>
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
            <Table :columns="columns" border :data="tableData"></Table>
            <Page :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </Card>
        <Modal v-model="showModal" :title="$t('Public.Create')">
            <div>
                <Form ref="newForm" label-position="top" :rules="newRule" :model="createModel">
                    <slot name="newform" v-bind:createModel="createModel" v-bind:rules="newRule"></slot>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showModal=false">{{$t('Public.Cancel')}}</Button>
                <Button @click="create" type="primary">{{$t('Public.Save')}}</Button>
            </div>
        </Modal>
        <Modal v-model="showEditModal" :title="$t('Public.Edit')">
            <div>
                <Form ref="productForm" label-position="top" :rules="editRule" :model="editModel">
                    <slot name="editform" v-bind:editModel="editModel"></slot>
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
export default {
  props: {
    title: {
      type: String
    },
    columnsetting: {
      type: Object
    },
    api: {
      type: Object
    },
    newRule: {
      type: Object
    },
    editRule: {
      type: Object
    },
    createFormat:{
      type: Function
    }
  },
  methods: {
    async create() {
      var _this = this;
      this.$refs.newForm.validate(async val => {
        if (val) {
          await _this.api.Create(_this.createModel);
          _this.showModal = false;
          await _this.getpage();
        }
      });
    },
    async edit() {
      var _this = this;
      this.$refs.productForm.validate(async val => {
        if (val) {
          await _this.api.Update(_this.editModel);
          _this.showEditModal = false;
          await _this.getpage();
        }
      });
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
        skipCount: (this.state.currentPage - 1) * this.state.pageSize
      };
      let rep = await this.api.Search({ params: page });
      this.state.tableData = [];
      this.state.tableData.push(...rep.data.result.items);
      this.state.totalCount = rep.data.result.totalCount;
    },
    handleClickActionsDropdown(name) {
      if (name === "Create") {
        if(this.createFormat){
          this.createModel = this.createFormat();
          this.$refs.newForm.resetFields();
        }
        else{
          this.createModel = {};
        }
        this.showModal = true;
      } else if (name === "Refresh") {
        this.getpage();
      }
    }
  },
  data() {
    var cm = {};
    if (this.columnsetting.needAction) {
      this.columnsetting.columns.push({
        title: this.$t('Public.Actions'),
        key: "action",
        width: 150,
        render: (h, params) => {
          return h("div", [
            h(
              "Button",
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
                    this.editModel = params.row;
                    this.showEditModal = true;
                  }
                }
              },
              this.$t('Public.Edit')
            ),
            h(
              "Button",
              {
                props: {
                  type: "error",
                  size: "small"
                },
                on: {
                  click: async () => {
                    this.$Modal.confirm({
                      title: this.$t(''),
                      content: this.$t('Public.Delete') + this.$t(this.title),
                      okText: this.$t('Public.Yes'),
                      cancelText: this.$t('Public.No'),
                      onOk: async () => {
                        await this.api.Delete(params.row.id);
                        // await this.getpage();
                      }
                    });
                  }
                }
              },
              this.$t('Public.Delete')
            )
          ]);
        }
      });
    }
    if(this.createFormat){
      cm = this.createFormat();
    } 
    return {
      columns: this.columnsetting.columns,
      editModel: {},
      createModel: cm,
      showModal: false,
      showEditModal: false,
      state: {
        tableData: [],
        totalCount: 0,
        pageSize: 10,
        currentPage: 1
      }
    };
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
  async created() {
    this.getpage();
  }
};
</script>