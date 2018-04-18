<template>
    <div>
        <Card>
            <p slot="title">{{title|l}}</p>
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
            <Table :columns="columns" border :data="tableData"></Table>
            <Page :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
        </Card>
        <Modal v-model="showModal" :title="L('Create')">
            <div>
                <Form ref="newForm" label-position="top" :rules="newRule" :model="createModel">
                    <slot name="newform" v-bind:createModel="createModel"></slot>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showModal=false">{{'Cancel'|l}}</Button>
                <Button @click="create" type="primary">{{'Save'|l}}</Button>
            </div>
        </Modal>
        <Modal v-model="showEditModal" :title="L('Edit')">
            <div>
                <Form ref="productForm" label-position="top" :rules="editRule" :model="editModel">
                    <slot name="editform" v-bind:editModel="editModel"></slot>
                </Form>
            </div>
            <div slot="footer">
                <Button @click="showEditModal=false">{{'Cancel'|l}}</Button>
                <Button @click="edit" type="primary">{{'Save'|l}}</Button>
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
    }
  },
  methods: {
    async create() {
      this.$refs.newForm.validate(async val => {
        if (val) {
          await this.api.Create(this.createModel);
          this.showModal = false;
          await this.getpage();
        }
      });
    },
    async edit() {
      this.$refs.productForm.validate(async val => {
        if (val) {
          await this.api.Update(this.editModel);
          this.showEditModal = false;
          await this.getpage();
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
        this.showModal = true;
      } else if (name === "Refresh") {
        this.getpage();
      }
    }
  },
  data() {
    if (this.columnsetting.needAction) {
      this.columnsetting.columns.push({
        title: this.L("Actions"),
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
              this.L("Edit")
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
                      title: this.L(""),
                      content: this.L("Delete product"),
                      okText: this.L("Yes"),
                      cancelText: this.L("No"),
                      onOk: async () => {
                        await this.api.Delete(params.row.id);
                        await this.getpage();
                      }
                    });
                  }
                }
              },
              this.L("Delete")
            )
          ]);
        }
      });
    }
    return {
      columns: this.columnsetting.columns,
      editModel: {},
      createModel: {},
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