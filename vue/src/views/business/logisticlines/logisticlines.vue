<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newLogisticLineRule"
        :editRule="logisticLineRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('LineName')" prop="lineName">
                        <Input v-model="slotProps.createModel.lineName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="L('LineCode')" prop="lineCode">
                        <Input v-model="slotProps.createModel.lineCode" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="L('Logistic')" prop="logisticId">
                        <Input v-model="slotProps.createModel.logisticId" :maxlength="50"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('LineName')" prop="lineName">
                        <Input v-model="slotProps.editModel.lineName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="L('LineCode')" prop="lineCode">
                        <Input v-model="slotProps.editModel.lineCode" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="L('Logistic')" prop="logisticId">
                        <Input v-model="slotProps.editModel.logisticId" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{'IsActive'|l}}</Checkbox>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import LogisticLineApi from "@/api/logisticline";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "LogisticLine",
      api: LogisticLineApi,
      newLogisticLineRule: {
        lineName: [{ require: true }],
        lineCode: [{ require: true }],
        logisticId: [{ require: true }]
      },
      logisticLineRule: {
        lineName: [{ require: true }],
        lineCode: [{ require: true }],
        logisticId: [{ require: true }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.L("LineName"),
            key: "lineName"
          },
          {
            title: this.L("LineCode"),
            key: "lineCode"
          },
          {
            title: this.L("LogisticId"),
            key: "logisticId"
          },
          {
            title: this.L("IsActive"),
            render: (h, params) => {
              return h("Checkbox", {
                props: {
                  value: params.row.isActive,
                  disabled: true
                }
              });
            }
          }
        ]
      }
    };
  }
};
</script>